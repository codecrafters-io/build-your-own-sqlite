The entry point for your SQLite implementation is in `app/main.py`.

Study and uncomment the relevant code: 

```python
# Uncomment this to pass the first stage
database_file.seek(16)  # Skip the first 16 bytes of the header
page_size = int.from_bytes(database_file.read(2), byteorder="big")
print(f"database page size: {page_size}")
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
