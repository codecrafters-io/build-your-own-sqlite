import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.file.Files;
import java.nio.file.Path;

public class Main {
  public static void main(String[] args){
    if (args.length < 2) {
      System.out.println("Missing <database path> and <command>");
      return;
    }

    String databaseFilePath = args[0];
    String command = args[1];

    switch (command) {
      case ".dbinfo" -> {
        try {
          byte[] header = Files.readAllBytes(Path.of(databaseFilePath));

          // The page size is stored at the 16th byte offset, using 2 bytes in big-endian order.
          // '& 0xFFFF' is used to convert the signed short to an unsigned int.
          int pageSize = ByteBuffer.wrap(header).order(ByteOrder.BIG_ENDIAN).position(16).getShort() & 0xFFFF;

          // You can use print statements as follows for debugging, they'll be visible when running tests.
          System.out.println("Logs from your program will appear here!");

          // Uncomment this block to pass the first stage
          // System.out.println("database page size: " + pageSize);
        } catch (IOException e) {
          System.out.println("Error reading file: " + e.getMessage());
        }
      }
      default -> System.out.println("Missing or invalid command passed: " + command);
    }
  }
}
