class Database
  def initialize(database_file_path)
    @database_file_path = database_file_path
  end

  def page_size
    File.open(@database_file_path, "rb") do |database_file|
      # You can use print statements as follows for debugging, they'll be visible when running tests.
      puts "Logs from your program will appear here"

      # Uncomment this to pass the first stage
      # database_file.seek(16)  # Skip the first 16 bytes of the header
      # database_file.read(2).unpack("n")[0]
    end
  end
end

database_file_path = ARGV[0]
command = ARGV[1]
if command == ".dbinfo"
  page_size = Database.new(database_file_path).page_size
  puts "database page size: #{page_size}"
end