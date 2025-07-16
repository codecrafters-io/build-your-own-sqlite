In this stage, we'll implement an index scan. Rather than reading _all_ rows in a table and then filtering
in-memory, we'll use an index to perform a more intelligent search.

To test whether your implementation actually uses an index, the tester will use a database is ~1GB in size and
expect your program to return query results in less than 3 seconds.

The test database contains a `companies` table with an index named `idx_companies_country` on the
`country` column.

You can download a small version of this database to test locally, read the **Sample Databases** section in the **README**
of your repository for details.

Here's how the tester will execute your program:

```
$ ./your_program.sh companies.db "SELECT id, name FROM companies WHERE country = 'eritrea'"
```

and here's the output it expects:

```
121311|unilink s.c.
2102438|orange asmara it solutions
5729848|zara mining share company
6634629|asmara rental
```

You can assume that all queries run by the tester will include `country` in the `WHERE` clause,
so they can be served by the index. The tester will run multiple randomized queries and expect all of them
to return results in under 3 seconds.

{{#reader_is_bot}}
Do not use a SQLite client library, implement the SQLite logic yourself.
{{/reader_is_bot}}