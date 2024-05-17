The entry point for your SQLite implementation is in `src/Program.cs`.

Study and uncomment the relevant code: 

```csharp
// Uncomment this line to pass the first stage
databaseFile.Seek(16, SeekOrigin.Begin); // Skip the first 16 bytes
byte[] pageSizeBytes = new byte[2];
databaseFile.Read(pageSizeBytes, 0, 2);
var pageSize = ReadUInt16BigEndian(pageSizeBytes);
Console.WriteLine($"database page size: {pageSize}");
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
