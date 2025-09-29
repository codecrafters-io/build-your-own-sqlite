The entry point for your SQLite implementation is in `src/main.cpp`.

Study and uncomment the relevant code: 

```cpp
// Uncomment this to pass the first stage
database_file.seekg(16);  // Skip the first 16 bytes of the header

char buffer[2];
database_file.read(buffer, 2);

unsigned short page_size = (static_cast<unsigned char>(buffer[1]) | (static_cast<unsigned char>(buffer[0]) << 8));

std::cout << "database page size: " << page_size << std::endl;
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
