The entry point for your SQLite implementation is in `src/Program.cs`.

Study and uncomment the relevant code: 

```csharp
// Uncomment this line to pass the first stage
var pageSize = ReadUInt16BigEndian(database[16..17]);
Console.WriteLine($"database page size: {pageSize}");
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
