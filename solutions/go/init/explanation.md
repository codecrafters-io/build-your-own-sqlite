The entry point for your SQLite implementation is in `cmd/mysqlite3/main.go`.

Study and uncomment the relevant code: 

```go
// Uncomment this to pass the first stage
info, err := db.Info()
if err != nil {
	return fmt.Errorf("get db info: %w", err)
}

fmt.Printf("number of tables: %v\n", info.Tables)
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
