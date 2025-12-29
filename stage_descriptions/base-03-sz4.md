In this stage, you'll implement the `.tables` [dot command](https://www.sqlite.org/cli.html#special_commands_to_sqlite3_dot_commands_), which prints the names of the user tables in a SQLite database.

### The `sqlite_schema.tbl_name` column

The names of the tables in a SQLite database are stored in the `tbl_name` column of the [`sqlite_schema`](https://www.sqlite.org/schematab.html) table. The `sqlite_schema` [page](https://www.sqlite.org/fileformat.html#b_tree_pages) stores the rows of the `sqlite_schema` table in chunks of data called "cells." Each cell contains a single row. You need to read all the cells and extract the value of `sqlite_schema.tbl_name` from each one.

### Cell pointer array

To figure out where the cells are located, read the `sqlite_schema` page's cell pointer array. This array specifies the offsets of every cell on the page. Here's what you need to know:

- The array appears directly after the page header.
- The elements (offsets) are 2-byte big-endian values.
- The offsets are relative to the start of the page.
- The array size is equal to the number of cells on the page (The page header specifies the number of cells on the page).

### Cell

Once you have all the offsets, you can read the cells. The type of cell on the `sqlite_schema` page is called a "table b-tree leaf cell." It's made up of three parts:

1. The size of the record, in bytes (varint)
2. The rowid (varint)
3. The record (record format)

Cells use variable-length integers, also called "varints." See the [official documentation](https://www.sqlite.org/fileformat.html#b_tree_pages) to learn how they work.

You can ignore the rowid—it's not relevant to this stage.

The part you're interested in is the record. "Record" is just another word for "row." That's the part that contains the `sqlite_schema.tbl_name` column.

#### Record format

Records are stored in [record format](https://www.sqlite.org/fileformat.html#record_format):

1. Header:
    1. Size of the header, including this value (varint)
    2. Serial type code for each column in the record, in order (varint)
2. Body:
    1. The value of each column in the record, in order (format varies based on serial type code)

A "serial type code" specifies the data type and size of a column. See the [official documentation](https://www.sqlite.org/fileformat.html#record_format) for the table of all serial type codes.

#### Example

The following is a cell from page 1 of `sample.db`:
```
00000ec0           78 03 07 17 1b  1b 01 81 47 74 61 62 6c  |   x.......Gtabl|
00000ed0  65 6f 72 61 6e 67 65 73  6f 72 61 6e 67 65 73 04  |eorangesoranges.|
00000ee0  43 52 45 41 54 45 20 54  41 42 4c 45 20 6f 72 61  |CREATE TABLE ora|
00000ef0  6e 67 65 73 0a 28 0a 09  69 64 20 69 6e 74 65 67  |nges.(..id integ|
00000f00  65 72 20 70 72 69 6d 61  72 79 20 6b 65 79 20 61  |er primary key a|
00000f10  75 74 6f 69 6e 63 72 65  6d 65 6e 74 2c 0a 09 6e  |utoincrement,..n|
00000f20  61 6d 65 20 74 65 78 74  2c 0a 09 64 65 73 63 72  |ame text,..descr|
00000f30  69 70 74 69 6f 6e 20 74  65 78 74 0a 29           |iption text.)   |
```

Here's an analysis of the cell:
```
// Size of the record (varint): 120
78

// The rowid (safe to ignore)
03

// Record header
07     // Size of record header (varint): 7

17     // Serial type for sqlite_schema.type (varint):     23
       // Size of sqlite_schema.type =                     (23-13)/2 = 5

1b     // Serial type for sqlite_schema.name (varint):     27
       // Size of sqlite_schema.name =                     (27-13)/2 = 7

1b     // Serial type for sqlite_schema.tbl_name (varint): 27
       // Size of sqlite_schema.tbl_name =                 (27-13)/2 = 7

01     // Serial type for sqlite_schema.rootpage (varint): 1
       // 8-bit twos-complement integer

81 47  // Serial type for sqlite_schema.sql (varint):      199
       // Size of sqlite_schema.sql =                      (199-13)/2 = 93

// Record body
74 61 62 6c 65        // Value of sqlite_schema.type:     "table"
6f 72 61 6e 67 65 73  // Value of sqlite_schema.name:     "oranges"
6f 72 61 6e 67 65 73  // Value of sqlite_schema.tbl_name: "oranges"  <---
...
```

### Tests

Here's how the tester will execute your program:
```
$ ./your_sqlite3.sh sample.db .tables
```

Your program must print the names of the tables in the database file:
```
apples oranges
```

### Notes

- The actual `.tables` command accepts an optional pattern argument, and also adds additional spaces between each table name, for formatting purposes. You do not need to implement either of these features for your `.tables` command.
- If a cell's payload is too large to fit on a single page, the remainder of the payload will be stored on [cell payload overflow pages](https://www.sqlite.org/fileformat.html#cell_payload_overflow_pages). You do not need to handle payload overflow in this challenge.
- The record part of a cell is called "payload," in the official documentation.

{{#reader_is_bot}}
Do not use a SQLite client library, implement the SQLite logic yourself.
{{/reader_is_bot}}
