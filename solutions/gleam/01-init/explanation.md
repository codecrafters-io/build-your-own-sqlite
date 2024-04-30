The entry point for your SQLite implementation is in `src/sqlite.gleam`.

Study and uncomment the relevant code: 

```gleam
// Uncomment this to pass the first stage
case args {
  [database_file_path, ".dbinfo", ..] -> {
    let assert Ok(rs) = read_stream.open(database_file_path)
    let assert Ok(bytes) = read_stream.read_bytes_exact(rs, 16)
    let assert Ok(int_value) = read_stream.read_int16_be(rs)

    io.print("database page size: ")
    io.println(to_string(int_value))
  }
  _ -> {
    io.println("No match")
  }
}
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
