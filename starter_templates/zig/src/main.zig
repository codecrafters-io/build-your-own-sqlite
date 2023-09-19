const std = @import("std");
const fs = std.fs;
const io = std.io;

pub fn main() !void {
    var args = std.process.args();
    if (args.len < 3) {
        try std.io.getStdErr().writer().print("Usage: {s} <database_file_path> <command>\n", .{args[0]});
        return null;
    }

    const database_file_path = args[1];
    const command = args[2];

    if (std.mem.eql(u8, command, ".dbinfo")) {
        const file = try fs.cwd().openFile(database_file_path, .{});
        defer file.close();
        
        try std.io.getStdOut().writer().print("Logs from your program will appear here\n", .{});
        
        // Uncomment this to pass the first stage
        // 
        // try file.seekBy(16);
        //
        // var buffer: [2]u8 = undefined;
        // try file.readExact(&buffer);
        //
        // const page_size = @intCast(u16, @bytesToSlice(u8, buffer[0..2])[0]) | @bytesToSlice(u8, buffer[0..2])[1] << 8;
        //
        // try std.io.getStdOut().writer().print("database page size: {}\n", .{page_size});
    }
}
