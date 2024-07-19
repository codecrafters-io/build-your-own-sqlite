The entry point for your SQLite implementation is in `src/sqlite.gleam`.

Study and uncomment the relevant code: 

```gleam
// Uncomment this to pass the first stage
case args {
  [database_file_path, ".dbinfo", ..] -> {
    let assert Ok(rs) = read_stream.open(database_file_path)
    // Skip the first 16 bytes
    let assert Ok(_bytes) = read_stream.read_bytes_exact(rs, 16)
    // The next 2 bytes hold the page size in big-endian format
    let assert Ok(page_size) = read_stream.read_uint16_be(rs)

    io.print("database page size: ")
    io.println(to_string(page_size))
  }
  _ -> {
    io.println("Unknown command")
  }
}
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
