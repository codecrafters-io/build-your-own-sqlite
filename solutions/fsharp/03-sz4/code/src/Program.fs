open System
open System.IO
open System.Buffers.Binary
open System.Text

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
    | _ -> failwith $"Invalid command: {command}"
