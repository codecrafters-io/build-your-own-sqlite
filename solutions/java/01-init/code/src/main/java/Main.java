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

    switch (args[1]) {
      case ".dbinfo" -> {
        try {
          byte[] header = Files.readAllBytes(Path.of(args[0]));
          // The page size is stored at the 16th byte offset, using 2 bytes in big-endian order
          int pageSize = ByteBuffer.wrap(header).order(ByteOrder.BIG_ENDIAN).position(16).getShort();

          System.out.println("database page size: " + pageSize);
        } catch (IOException e) {
          System.out.println("Error reading file: " + e.getMessage());
        }
      }
      default -> System.out.println("Missing or invalid command passed: " + args[1]);
    }
  }
}
