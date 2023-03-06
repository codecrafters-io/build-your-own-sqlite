import Foundation

@main
struct Main {

    static func main() async throws {
        switch CommandLine.argc {
        case 0, 1:
            print("Missing <database path> and <command>")
            return
        case 2:
            print("Missing <command>")
            return
        default:
            break
        }

        let path = CommandLine.arguments[1]
        let command = CommandLine.arguments[2]

        // Read database file as byte array.
        let database = try Data(contentsOf: URL(fileURLWithPath: path))

        // Parse command and perform required actions.
        switch command {
        case ".dbinfo":
            // You can use print statements as follows for debugging, they'll be visible when running tests.
            print("Logs from your program will appear here!")

            // Uncomment this block to pass the first stage
            // let pageSize = UInt16(database[16..<18])
            // print("database page size: \(pageSize)")
        default:
            fatalError("Invalid command \(command)")
        }
    }
}

// This could be put in a different file, this here only to make template simple.
extension UnsignedInteger {

    init<T: Collection>(_ bytes: T) where T.Element == UInt8 {
        precondition(
            bytes.count <= MemoryLayout<Self>.size,
            "bytes count: \(bytes.count), layout: \(MemoryLayout<Self>.size)"
        )

        var value = UInt64()

        for byte in bytes {
            value <<= 8
            value |= UInt64(byte)
        }

        self.init(value)
    }
}
