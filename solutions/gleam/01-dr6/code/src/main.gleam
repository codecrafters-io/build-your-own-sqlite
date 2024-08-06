import argv
import file_streams/read_stream
import gleam/int.{to_string}
import gleam/io

pub fn main() {
  let args = argv.load().arguments

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
}
