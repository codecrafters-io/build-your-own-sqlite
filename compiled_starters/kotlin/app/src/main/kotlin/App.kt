import java.nio.ByteBuffer
import kotlin.system.exitProcess

fun main(args: Array<String>) {
    // You can use print statements as follows for debugging, they'll be visible when running tests.
    System.err.println("Logs from your program will appear here!")

    if (args.size != 2) {
        println("Usage: ./your_program.sh <database path> <command>")
        exitProcess(1)
    }

    val databaseFilePath = args[0]
    val command = args[1]

    if (command == ".dbinfo") {
        val databaseFile = java.io.File(databaseFilePath).inputStream()

        // Uncomment this block to pass the first stage
        //
        // databaseFile.skip(16) // Skip the first 16 bytes of the header
        // val pageSizeBytes = ByteArray(2) // The following 2 bytes are the page size
        // databaseFile.read(pageSizeBytes)
        // val pageSize = ByteBuffer.wrap(pageSizeBytes).short.toUShort()
        // println("database page size: $pageSize")

        databaseFile.close()
    } else {
        println("Invalid command: $command")
    }
}
