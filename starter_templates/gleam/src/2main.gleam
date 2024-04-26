import gleam/io/file as file
import gleam/io/console as console

pub fn main(argv: List(String)) {
    let database_file_path = argv.0
    let command = argv.1

    // You can use print statements as follows for debugging, they'll be visible when running tests.
    io.println("Logs from your program will appear here!")

    // Uncomment this block to pass the first stage
    // if command == ".dbinfo" {
    //     match file.open(database_file_path, file.Mode.Read) {
    //         Ok(database_file) => {
    //             let mut header_skip_bytes = ByteArray(16)
    //             let _ = file.read_exact(database_file, header_skip_bytes)
    //             let page_size_bytes = ByteArray(2)
    //             let _ = file.read_exact(database_file, page_size_bytes)
    //             let page_size = U16.from_big_endian_bytes(page_size_bytes)
    //             console.log(format("database page size: {}", [page_size]))
    //         }
    //         Err(_err) => {
    //             console.log("Error: Unable to open database file.")
    //         }
    //     }
    // } else {
    //     console.log(format("Invalid command: {}", [command]))
    // }
}
