The entry point for your SQLite implementation is in `lib/main.ex`.

Study and uncomment the relevant code: 

```elixir
# Uncomment this to pass the first stage
:file.position(file, 16) # Skip the first 16 bytes of the header
<<page_size::16>> = IO.binread(file, 2)
IO.puts("database page size: #{page_size}")
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
