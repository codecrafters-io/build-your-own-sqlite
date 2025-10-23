using static System.Buffers.Binary.BinaryPrimitives;

// Parse arguments
var (path, command) = args.Length switch
{
    0 => throw new InvalidOperationException("Missing <database path> and <command>"),
    1 => throw new InvalidOperationException("Missing <command>"),
    _ => (args[0], args[1])
};

var databaseFile = File.OpenRead(path);

// Parse command and act accordingly
if (command == ".dbinfo")
{
    // You can use print statements as follows for debugging, they'll be visible when running tests.
    Console.Error.WriteLine("Logs from your program will appear here!");

    // Uncomment this line to pass the first stage
    // databaseFile.Seek(16, SeekOrigin.Begin); // Skip the first 16 bytes
    // byte[] pageSizeBytes = new byte[2];
    // databaseFile.Read(pageSizeBytes, 0, 2);
    // var pageSize = ReadUInt16BigEndian(pageSizeBytes);
    // Console.WriteLine($"database page size: {pageSize}");
}
else
{
    throw new InvalidOperationException($"Invalid command: {command}");
}
