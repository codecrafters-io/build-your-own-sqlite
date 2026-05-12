const std = @import("std");

pub fn main(init: std.process.Init) !void {
    const io = init.io;
    var stderr = std.Io.File.stderr().writer(io, &.{});

    const args = try std.process.Args.toSlice(init.minimal.args, init.arena.allocator());

    if (args.len < 3) {
        try stderr.interface.print("Usage: {s} <database_file_path> <command>\n", .{args[0]});
        std.process.exit(1);
    }

    const database_file_path: []const u8 = args[1];
    const command: []const u8 = args[2];

    if (std.mem.eql(u8, command, ".dbinfo")) {
        var file = try std.Io.Dir.cwd().openFile(io, database_file_path, .{});
        defer file.close(io);

        var reader_buf: [4096]u8 = undefined;
        var reader = file.reader(io, &reader_buf);
        try reader.seekTo(16);
        const page_size = std.mem.readInt(u16, try reader.interface.takeArray(2), .big);

        var stdout = std.Io.File.stdout().writer(io, &.{});
        try stdout.interface.print("database page size: {}\n", .{page_size});
    }
}
