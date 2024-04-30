import argv
import file_streams/read_stream
import gleam/bit_array
import gleam/int.{to_string}
import gleam/io
import gleam/list.{each}

pub fn main() {
  let args = argv.load().arguments
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
}
