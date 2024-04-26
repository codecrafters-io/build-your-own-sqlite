The entry point for your SQLite implementation is in `src/sqlite.gleam`.

Study and uncomment the relevant code: 

```gleam
// Uncomment this block to pass the first stage
pub fn main() {
  let args = io.argv()

  case args {
    [_, database_file_path, command] -> {
      case command {
        ".dbinfo" -> {
          io.open(database_file_path, io.Read)
          |> result.then(fn(file) {
            io.seek(file, 16)
            |> result.then(fn(_) {
              io.read_bytes(file, 2)
              |> result.then(fn(bytes) {
                let page_size = bytes
                  |> list.reverse
                  |> int.from_bytes_be
                io.println(string.concat(["database page size: ", int.to_string(page_size)]))
                |> result.map(fn(_) { io.close(file) })
              })
            })
          })
          |> result.unwrap(crash)
        }
        _ -> io.println(string.concat(["Invalid command: ", command]))
      }
    }
    _ -> io.println("Usage: gleam run database_file_path command")
  }
}
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
