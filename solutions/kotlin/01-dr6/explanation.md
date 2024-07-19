The entry point for your SQLite implementation is in `src/main/kotlin/Main.kt`.

Study and uncomment the relevant code: 

```kotlin
// Uncomment this block to pass the first stage

databaseFile.skip(16) // Skip the first 16 bytes of the header
val pageSizeBytes = ByteArray(2) // The following 2 bytes are the page size
databaseFile.read(pageSizeBytes)
val pageSize = java.nio.ByteBuffer.wrap(pageSizeBytes).short.toInt()
println("database page size: $pageSize")
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
