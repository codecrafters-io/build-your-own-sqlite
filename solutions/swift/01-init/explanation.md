The entry point for your SQLite implementation is in `Sources/swift-sqlite-challenge/Main.swift`.

Study and uncomment the relevant code: 

```swift
// Uncomment this block to pass the first stage
let pageSize = UInt16(database[16..<18])
print("database page size: \(pageSize)")
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
