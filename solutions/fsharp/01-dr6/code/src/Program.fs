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
        databaseFile.Seek(16L, SeekOrigin.Begin) |> ignore // Skip the first 16 bytes
        let pageSizeBytes = Array.zeroCreate<byte> 2
        databaseFile.ReadExactly(pageSizeBytes, 0, 2)
        let pageSize = BinaryPrimitives.ReadUInt16BigEndian(pageSizeBytes)
        printfn $"database page size: {pageSize}"
        0
    | _ -> failwith $"Invalid command: {command}"
