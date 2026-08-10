open System
open System.IO
open System.Buffers.Binary
open System.Text
open System.Text.RegularExpressions

// A value stored in a column of a record
type Value =
    | Null
    | Integer of int64
    | Real of float
    | Text of string
    | Blob of byte[]

// Read a big-endian unsigned 16-bit integer at the given offset
let readUInt16 (bytes: byte[]) (offset: int) =
    BinaryPrimitives.ReadUInt16BigEndian(ReadOnlySpan(bytes, offset, 2))

// Read a SQLite varint starting at the given offset.
// A varint is 1-9 bytes long: the lower 7 bits of each byte are used, and the
// high bit signals that another byte follows. The 9th byte (if present)
// contributes all 8 bits. Returns the value and the number of bytes consumed.
let readVarint (bytes: byte[]) (offset: int) =
    let rec go acc i =
        let b = bytes[offset + i]
        if i = 8 then (acc <<< 8) ||| int64 b, 9
        elif b < 0x80uy then (acc <<< 7) ||| int64 b, i + 1
        else go ((acc <<< 7) ||| int64 (b &&& 0x7Fuy)) (i + 1)

    go 0L 0

// The size in bytes of a value with the given serial type code
let serialTypeSize (serialType: int64) =
    match serialType with
    | 0L | 8L | 9L -> 0
    | 1L -> 1
    | 2L -> 2
    | 3L -> 3
    | 4L -> 4
    | 5L -> 6
    | 6L | 7L -> 8
    | n when n >= 12L && n % 2L = 0L -> int (n - 12L) / 2 // BLOB
    | n when n >= 13L -> int (n - 13L) / 2 // TEXT
    | n -> failwith $"Invalid serial type: {n}"

// Read a big-endian signed integer of the given size at the given offset
let readBigEndianInt (bytes: byte[]) (offset: int) (size: int) =
    let mutable value = if bytes[offset] >= 0x80uy then -1L else 0L // Sign-extend

    for i in 0 .. size - 1 do
        value <- (value <<< 8) ||| int64 bytes[offset + i]

    value

// Read a value with the given serial type code at the given offset
let readSerialValue (bytes: byte[]) (offset: int) (serialType: int64) =
    match serialType with
    | 0L -> Null
    | 1L | 2L | 3L | 4L | 5L | 6L -> Integer(readBigEndianInt bytes offset (serialTypeSize serialType))
    | 7L -> Real(BitConverter.Int64BitsToDouble(readBigEndianInt bytes offset 8))
    | 8L -> Integer 0L
    | 9L -> Integer 1L
    | n when n % 2L = 0L -> Blob(bytes[offset .. offset + serialTypeSize n - 1])
    | n -> Text(Encoding.UTF8.GetString(bytes, offset, serialTypeSize n))

// Read a record (one row) in SQLite's record format at the given offset.
// The record header lists a serial type for each column, and the body contains
// the values in the same order.
let readRecord (bytes: byte[]) (offset: int) =
    let headerSize, headerSizeLength = readVarint bytes offset

    // Read serial types until the header is exhausted
    let serialTypes =
        let mutable pos = offset + headerSizeLength

        [| while pos < offset + int headerSize do
               let serialType, length = readVarint bytes pos
               pos <- pos + length
               yield serialType |]

    // Read the values, each one starting where the previous one ended
    let mutable pos = offset + int headerSize

    [| for serialType in serialTypes do
           yield readSerialValue bytes pos serialType
           pos <- pos + serialTypeSize serialType |]

// A b-tree page, with the raw page data and the parsed page header
type Page =
    { Data: byte[]
      PageType: byte
      CellPointers: int[] }

// Read the page with the given number (page numbers start at 1)
let readPage (databaseFile: FileStream) (pageSize: int) (pageNumber: int) =
    let data = Array.zeroCreate<byte> pageSize
    databaseFile.Seek(int64 (pageNumber - 1) * int64 pageSize, SeekOrigin.Begin) |> ignore
    databaseFile.ReadExactly(data, 0, pageSize)

    // Page 1 contains the 100-byte file header, the page header comes after it
    let headerOffset = if pageNumber = 1 then 100 else 0
    let pageType = data[headerOffset]
    let cellCount = int (readUInt16 data (headerOffset + 3))

    // Interior pages have a 12-byte header, leaf pages an 8-byte one. The cell
    // pointer array follows the header: 2 bytes per cell, relative to the
    // start of the page.
    let pageHeaderSize = if pageType = 0x02uy || pageType = 0x05uy then 12 else 8

    let cellPointers =
        [| for i in 0 .. cellCount - 1 -> int (readUInt16 data (headerOffset + pageHeaderSize + i * 2)) |]

    { Data = data
      PageType = pageType
      CellPointers = cellPointers }

// Read the record stored in a table b-tree leaf cell at the given offset.
// A leaf table cell is: payload size (varint), rowid (varint), record.
let readLeafTableCell (page: Page) (cellPointer: int) =
    let _payloadSize, payloadSizeLength = readVarint page.Data cellPointer
    let rowid, rowidLength = readVarint page.Data (cellPointer + payloadSizeLength)
    rowid, readRecord page.Data (cellPointer + payloadSizeLength + rowidLength)

// Read the rows of the sqlite_schema table, which are stored on page 1.
// Each row is: type, name, tbl_name, rootpage, sql
let readSchemaRows (databaseFile: FileStream) (pageSize: int) =
    let page = readPage databaseFile pageSize 1
    [| for cellPointer in page.CellPointers -> snd (readLeafTableCell page cellPointer) |]

let textValue (value: Value) =
    match value with
    | Text text -> text
    | value -> failwith $"Expected a text value, got: {value}"

let integerValue (value: Value) =
    match value with
    | Integer integer -> integer
    | value -> failwith $"Expected an integer value, got: {value}"

// Find the sqlite_schema row for the given table.
// Each row is: type, name, tbl_name, rootpage, sql
let findTableSchemaRow (databaseFile: FileStream) (pageSize: int) (tableName: string) =
    readSchemaRows databaseFile pageSize
    |> Array.tryFind (fun row -> row[0] = Text "table" && textValue row[2] = tableName)
    |> function
        | Some row -> row
        | None -> failwith $"Table not found: {tableName}"

// A column definition from a CREATE TABLE statement. Columns declared as
// "integer primary key" are aliases for the rowid: their values are stored as
// NULL in the record, and the actual value is the cell's rowid.
type Column = { Name: string; IsRowIdAlias: bool }

// Extract the ordered column definitions from a CREATE TABLE statement
let parseCreateTableColumns (sql: string) =
    // The column definitions are the comma-separated list between the outermost parentheses
    let columnDefs = sql[sql.IndexOf('(') + 1 .. sql.LastIndexOf(')') - 1]

    // Split on commas, ignoring those nested in parentheses (e.g. "varchar(255)")
    let defs =
        let mutable depth = 0
        let mutable current = StringBuilder()

        [ for c in columnDefs do
              match c with
              | '(' ->
                  depth <- depth + 1
                  current.Append(c) |> ignore
              | ')' ->
                  depth <- depth - 1
                  current.Append(c) |> ignore
              | ',' when depth = 0 ->
                  yield current.ToString()
                  current <- StringBuilder()
              | c -> current.Append(c) |> ignore

          yield current.ToString() ]

    // The column name is the first token of each definition. Definitions
    // starting with a constraint keyword are table constraints, not columns.
    let constraintKeywords = [ "primary"; "foreign"; "unique"; "check"; "constraint" ]

    [| for def in defs do
           let def = def.Trim()
           let tokens = def.Split(' ', '\t', '\n', '\r')
           let name = tokens[0].Trim('"', '`', '[', ']', '\'')

           if not (List.contains (name.ToLowerInvariant()) constraintKeywords) then
               { Name = name
                 IsRowIdAlias = Regex.IsMatch(def, @"integer\s+primary\s+key", RegexOptions.IgnoreCase) } |]

// Format a value for query output
let formatValue (value: Value) =
    match value with
    | Null -> ""
    | Integer integer -> string integer
    | Real real -> string real
    | Text text -> text
    | Blob blob -> Encoding.UTF8.GetString(blob)

[<EntryPoint>]
let main args =
    // Parse arguments
    let path, command =
        match args with
        | [||] -> failwith "Missing <database path> and <command>"
        | [| _ |] -> failwith "Missing <command>"
        | _ -> args[0], args[1]

    use databaseFile = File.OpenRead(path)

    // The database page size is a 2-byte big-endian value at offset 16 of the file header
    databaseFile.Seek(16L, SeekOrigin.Begin) |> ignore // Skip the first 16 bytes
    let pageSizeBytes = Array.zeroCreate<byte> 2
    databaseFile.ReadExactly(pageSizeBytes, 0, 2)
    let pageSize = int (BinaryPrimitives.ReadUInt16BigEndian(pageSizeBytes))

    // Parse command and act accordingly
    match command with
    | ".dbinfo" ->
        printfn $"database page size: {pageSize}"

        // Each cell on page 1 is a row of sqlite_schema, i.e. a table
        // (assuming no indexes, views or triggers)
        let schemaRows = readSchemaRows databaseFile pageSize
        printfn $"number of tables: {schemaRows.Length}"
        0
    | ".tables" ->
        // Table names are stored in the tbl_name column (index 2) of sqlite_schema.
        // Tables prefixed with sqlite_ are internal to SQLite.
        let tableNames =
            readSchemaRows databaseFile pageSize
            |> Array.map (fun row -> textValue row[2])
            |> Array.filter (fun name -> not (name.StartsWith("sqlite_")))
            |> Array.sort

        printfn $"""{String.Join(" ", tableNames)}"""
        0
    | query when query.StartsWith("SELECT COUNT(*) FROM ", StringComparison.OrdinalIgnoreCase) ->
        // No need for a full-blown SQL parser yet: the table name is the last word
        let tableName = query.Split(' ') |> Array.last

        // The number of rows is the number of cells in the table's b-tree,
        // which fits entirely on the root page for now
        let schemaRow = findTableSchemaRow databaseFile pageSize tableName
        let page = readPage databaseFile pageSize (int (integerValue schemaRow[3]))
        printfn $"{page.CellPointers.Length}"
        0
    | query when query.StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase) ->
        // SELECT <column1>,<column2>,... FROM <table> [WHERE <column> = <literal>]
        let m =
            Regex.Match(
                query,
                @"^SELECT\s+([\w\s,]+?)\s+FROM\s+(\w+)(?:\s+WHERE\s+(\w+)\s*=\s*(?:'([^']*)'|(\d+)))?\s*$",
                RegexOptions.IgnoreCase
            )

        if not m.Success then
            failwith $"Unsupported query: {query}"

        let columnNames = m.Groups[1].Value.Split(',') |> Array.map (fun name -> name.Trim())
        let tableName = m.Groups[2].Value

        // The WHERE clause compares a column against a string or integer literal
        let whereFilter =
            if m.Groups[3].Success then
                let literal =
                    if m.Groups[4].Success then
                        Text m.Groups[4].Value
                    else
                        Integer(int64 m.Groups[5].Value)

                Some(m.Groups[3].Value, literal)
            else
                None

        // The order of a column's values in a record matches the order of the
        // columns in the CREATE TABLE statement (stored in sqlite_schema.sql)
        let schemaRow = findTableSchemaRow databaseFile pageSize tableName
        let columns = parseCreateTableColumns (textValue schemaRow[4])

        let columnIndexes =
            [| for columnName in columnNames ->
                   columns
                   |> Array.tryFindIndex (fun column -> column.Name = columnName)
                   |> function
                       | Some index -> index
                       | None -> failwith $"Column not found: {columnName}" |]

        // Resolve the WHERE column to its index in the record, and account for
        // rowid aliasing when reading any column's value
        let columnValue (rowid: int64) (record: Value[]) (columnIndex: int) =
            match record[columnIndex] with
            | Null when columns[columnIndex].IsRowIdAlias -> Integer rowid
            | value -> value

        let matchesFilter (rowid: int64) (record: Value[]) =
            match whereFilter with
            | None -> true
            | Some(columnName, literal) ->
                columns
                |> Array.tryFindIndex (fun column -> column.Name = columnName)
                |> function
                    | Some index -> columnValue rowid record index = literal
                    | None -> failwith $"Column not found: {columnName}"

        let page = readPage databaseFile pageSize (int (integerValue schemaRow[3]))

        for cellPointer in page.CellPointers do
            let rowid, record = readLeafTableCell page cellPointer

            if matchesFilter rowid record then
                let values =
                    [| for columnIndex in columnIndexes -> columnValue rowid record columnIndex |]

                printfn $"""{String.Join("|", Array.map formatValue values)}"""

        0
    | _ -> failwith $"Invalid command: {command}"
