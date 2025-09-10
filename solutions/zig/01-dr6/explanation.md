The entry point for your SQLite implementation is in `src/main.zig`.

Study and uncomment the relevant code: 

```zig
// Uncomment this block to pass the first stage
var buf: [2]u8 = undefined;
_ = try file.seekTo(16);
_ = try file.read(&buf);
const page_size = std.mem.readInt(u16, &buf, .big);
try stdout.print("database page size: {}\n", .{page_size});
try stdout.flush();
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
