In this stage, you'll add the number of tables to your `.dbinfo` output.

### The schema table

SQLite describes its tables in a table of its own,
[`sqlite_schema`](https://www.sqlite.org/schematab.html). Each table, index,
view or trigger has a row there, except `sqlite_schema` itself.

For this stage, databases contain only tables. Counting schema rows therefore
gives the table count, including internal tables such as `sqlite_sequence`. You
can inspect those rows with SQLite's own client:

```
$ sqlite3 sample.db "SELECT * FROM sqlite_schema;"
```

### Schema rows on page 1

A database is divided into
[pages](https://www.sqlite.org/fileformat.html#pages). SQLite organizes table
storage as [B-trees](https://www.sqlite.org/fileformat.html#b_tree_pages), whose
pages can hold rows or point to other pages. A tree's **root page** is its
starting page.

The root of `sqlite_schema` is always page 1. In this challenge, the schema fits
on that single page, so it is a **leaf page**: it stores rows directly. Each row
occupies a chunk of bytes called a **cell**. The page's cell count is therefore
the schema's row count.

Page 1 begins at file offset 0 and includes the 100-byte database header. Its
separate B-tree page header starts at offset 100. A table leaf page has an
8-byte header with these fields:

| Offset within the page header | Size    | Meaning                                 |
| ----------------------------- | ------- | --------------------------------------- |
| 0                             | 1 byte  | Page type: `0x0d` for a table leaf page |
| 3                             | 2 bytes | Number of cells, stored big-endian      |

The cell count is at file bytes 103–104 on page 1. In a multi-page table, each
page has its own cell count.

### Tests

The tester will run your program as before:

```
$ ./your_program.sh sample.db .dbinfo
```

Print both values to standard output:

```
database page size: 4096
number of tables: 3
```

These are the values for the supplied `sample.db`. The tester also uses other
databases, so both values must come from the file.

### Notes

- Inspect the bytes with `hexdump -C sample.db` or a hex editor such as
  [HexEd.it](https://hexed.it/). The file header and page header are adjacent
  but describe different things.

{{#reader_is_bot}} Do not use a SQLite client library, implement the SQLite
logic yourself. {{/reader_is_bot}}
