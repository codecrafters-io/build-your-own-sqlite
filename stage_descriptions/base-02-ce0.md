In this stage, you'll add "number of tables" to your `.dbinfo` command's output.

### The `sqlite_schema` table

To get the number of tables in a SQLite database, you need to examine the database's [`sqlite_schema`](https://www.sqlite.org/schematab.html) table. The `sqlite_schema` table stores the database schema.

For each table, index, view, or trigger in the database, there's a corresponding row in `sqlite_schema`. The one exception is that there's no row for the `sqlite_schema` table itself.

To see what `sqlite_schema` looks like, run this command:
```
$ sqlite3 sample.db "SELECT * FROM sqlite_schema;"
```

In this challenge, you can assume that databases only contain tables—no indexes, views, or triggers. So, each row in `sqlite_schema` represents a table in the database. As a result, you can get the total number of tables in the database by getting the number of rows in `sqlite_schema`.

### Pages

A SQLite database file is made up of one or more [pages](https://www.sqlite.org/fileformat.html#pages). All tables, including `sqlite_schema`, are stored on one or more [table b-tree pages](https://www.sqlite.org/fileformat.html#b_tree_pages).

In this challenge, you can assume that the `sqlite_schema` table is small enough to fit entirely on a single page. (In reality, it can sometimes span multiple pages.) In order to get the number of rows in `sqlite_schema`, you need to read the `sqlite_schema` page.

#### The `sqlite_schema` page

You'll learn more about b-tree pages in later stages. For now, here's what you need to know:
- The `sqlite_schema` page is always page 1, and it always begins at offset 0. The file header is a part of the page.
- The `sqlite_schema` page stores the rows of the `sqlite_schema` table in chunks of data called "cells." Each cell stores a single row.

So, the number of tables in the database is equal to the number of cells on the `sqlite_schema` page.

#### Cell count

You can get the number of cells on the `sqlite_schema` page by looking at the `sqlite_schema` page header. The b-tree page header contains a 2-byte big-endian value that specifies number of cells on the page. See the [official documentation](https://www.sqlite.org/fileformat.html#b_tree_pages) for more information.

Note that the page header is separate from the file header. The page header appears directly after the file header.

### Tests

Here's how the tester will execute your program:
```
$ ./your_program.sh sample.db .dbinfo
```

Your program must print the following values:
- Database page size
- Number of tables

```
database page size: 4096
number of tables: 3
```

### Notes

- You may find it useful to read through `sample.db` and make sure you understand the file format, before working on a solution. To do this, you can run `hexdump -C sample.db`, or use a hex editor like [HexEd.it](https://hexed.it/).
{{#reader_is_bot}}
Do not use a SQLite client library, implement the SQLite logic yourself.
{{/reader_is_bot}}