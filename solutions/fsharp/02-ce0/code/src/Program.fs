open System
open System.IO
open System.Buffers.Binary

[<EntryPoint>]
let main args =
    // Parse arguments
    let path, command =
        match args with
        | [||] -> failwith "Missing <database path> and <command>"
        | [| _ |] -> failwith "Missing <command>"
        | _ -> args[0], args[1]

    // Parse command and act accordingly
    match command with
    | ".dbinfo" ->
        use databaseFile = File.OpenRead(path)

        // The database page size is a 2-byte big-endian value at offset 16 of the file header
        databaseFile.Seek(16L, SeekOrigin.Begin) |> ignore // Skip the first 16 bytes
        let pageSizeBytes = Array.zeroCreate<byte> 2
        databaseFile.ReadExactly(pageSizeBytes, 0, 2)
        let pageSize = BinaryPrimitives.ReadUInt16BigEndian(pageSizeBytes)
        printfn $"database page size: {pageSize}"

        // Page 1 starts at offset 0, and the 100-byte file header is part of it.
        // The b-tree page header follows the file header, and the number of cells
        // is a 2-byte big-endian value at offset 3 of the page header. Each cell on
        // page 1 is a row of sqlite_schema, i.e. a table (assuming no indexes,
        // views or triggers).
        databaseFile.Seek(100L + 3L, SeekOrigin.Begin) |> ignore
        let cellCountBytes = Array.zeroCreate<byte> 2
        databaseFile.ReadExactly(cellCountBytes, 0, 2)
        let cellCount = BinaryPrimitives.ReadUInt16BigEndian(cellCountBytes)
        printfn $"number of tables: {cellCount}"
        0
    | _ -> failwith $"Invalid command: {command}"
