using static System.Buffers.Binary.BinaryPrimitives;

// Parse arguments
var (path, command) = args.Length switch
{
    0 => throw new InvalidOperationException("Missing <database path> and <command>"),
    1 => throw new InvalidOperationException("Missing <command>"),
    _ => (args[0], args[1])
};

// Read database file into database
var database = File.ReadAllBytes(path).AsMemory();

// Parse command and act accordingly
if (command == ".dbinfo")
{
    var pageSize = ReadUInt16BigEndian(database[16..17]);
    Console.WriteLine($"database page size: {pageSize}");
}
else
{
    throw new InvalidOperationException($"Invalid command: {command}");
}
