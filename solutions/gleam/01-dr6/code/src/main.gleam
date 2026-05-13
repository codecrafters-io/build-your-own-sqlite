import argv
import file_streams/file_stream
import gleam/int
import gleam/io

pub fn main() {
  let args = argv.load().arguments

  case args {
    [database_file_path, ".dbinfo", ..] -> {
      let assert Ok(stream) = file_stream.open_read(database_file_path)
      // Skip the first 16 bytes of the header
      let assert Ok(_skip) = file_stream.read_bytes_exact(stream, 16)
      // The next 2 bytes hold the page size in big-endian format
      let assert Ok(header) = file_stream.read_bytes_exact(stream, 2)
      let page_size = case header {
        <<hi, lo>> -> hi * 256 + lo
        _ -> 0
      }
      let assert Ok(Nil) = file_stream.close(stream)

      io.print("database page size: ")
      io.println(int.to_string(page_size))
    }
    _ -> {
      io.println("Unknown command")
    }
  }
}
