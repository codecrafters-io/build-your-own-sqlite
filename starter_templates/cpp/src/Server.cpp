#include <iostream>
#include <fstream>

int main(int argc, char* argv[]) {
    // You can use print statements as follows for debugging, they'll be visible when running tests.
    std::cout << "Logs from your program will appear here" << std::endl;

    if (argc != 3) {
        std::cerr << "Expected two arguments" << std::endl;
        return 1;
    }

    std::string database_file_path = argv[1];
    std::string command = argv[2];

    if (command == ".dbinfo") {
        std::ifstream database_file(database_file_path, std::ios::binary);
        if (!database_file) {
            std::cerr << "Failed to open the database file" << std::endl;
            return 1;
        }


        // Uncomment this to pass the first stage
        // database_file.seekg(16);  // Skip the first 16 bytes of the header
        //
        // char buffer[2];
        // database_file.read(buffer, 2);
        //
        // unsigned short page_size;
        // memcpy(&page_size, buffer, 2);
        //
        // std::cout << "Database page size: " << page_size << std::endl;
    }

    return 0;
}
