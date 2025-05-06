defmodule CLI do
  def main(args) do
    case args do
      [database_file_path, command] ->
        case command do
          ".dbinfo" ->
            file = File.open!(database_file_path, [:read, :binary])

            # You can use print statements as follows for debugging, they'll be visible when running tests.
            IO.puts(:stderr, "Logs from your program will appear here!")
            
            # Uncomment this to pass the first stage
            # :file.position(file, 16) # Skip the first 16 bytes of the header
            # <<page_size::16>> = IO.binread(file, 2)
            # IO.puts("database page size: #{page_size}")
        end
      _ ->
        IO.puts(:stderr, "Usage: your_program <database_file> <command>")
    end
  end
end
