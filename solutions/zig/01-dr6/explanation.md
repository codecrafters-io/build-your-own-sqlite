The entry point for your SQLite implementation is in `src/main.zig`.

Study and uncomment the relevant code: 

```zig
// Uncomment this to pass the first stage

var gpa = std.heap.GeneralPurposeAllocator(.{}){};
defer _ = gpa.deinit();
const allocator = gpa.allocator();

const args = try std.process.argsAlloc(allocator);
defer std.process.argsFree(allocator, args);


if (args.len < 3) {
    try std.io.getStdErr().writer().print("Usage: {s} <database_file_path> <command>\n", .{args[0]});
    return;
}

var database_file_path: []const u8 = args[1];
var command: []const u8 = args[2];

if (std.mem.eql(u8, command, ".dbinfo")) {
    var file = try std.fs.cwd().openFile(database_file_path, .{});
    defer file.close();

    var buf: [2]u8 = undefined;
    _ = try file.seekTo(16);
    _ = try file.read(&buf);
    const page_size = std.mem.readInt(u16, &buf, .Big);
    try std.io.getStdOut().writer().print("database page size: {}\n", .{page_size});
}
```

```zig
// Uncomment this to pass the first stage

var gpa = std.heap.GeneralPurposeAllocator(.{}){};
defer _ = gpa.deinit();
const allocator = gpa.allocator();

const args = try std.process.argsAlloc(allocator);
defer std.process.argsFree(allocator, args);


if (args.len < 3) {
    try std.io.getStdErr().writer().print("Usage: {s} <database_file_path> <command>\n", .{args[0]});
    return;
}

var database_file_path: []const u8 = args[1];
var command: []const u8 = args[2];

if (std.mem.eql(u8, command, ".dbinfo")) {
    var file = try std.fs.cwd().openFile(database_file_path, .{});
    defer file.close();

    var buf: [2]u8 = undefined;
    _ = try file.seekTo(16);
    _ = try file.read(&buf);
    const page_size = std.mem.readInt(u16, &buf, .Big);
    try std.io.getStdOut().writer().print("database page size: {}\n", .{page_size});
}
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
