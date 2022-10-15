package sqlite

import (
	"fmt"
	"os"

	"github/com/codecrafters-io/sqlite-starter-go/dbfile"
)

type (
	DB struct {
		f *os.File

		dbHeader dbfile.DBHeader

		pageSize int64
	}

	Info struct {
		Tables int
	}
)

const Magic = "SQLite format 3"

func Open(file string) (*DB, error) {
	f, err := os.OpenFile(file, os.O_RDONLY, 0644)
	if err != nil {
		return nil, fmt.Errorf("open file: %w", err)
	}

	h, err := dbfile.ReadDBHeader(f)
	if err != nil {
		return nil, fmt.Errorf("read db header: %w", err)
	}

	if h.Magic() != Magic {
		return nil, fmt.Errorf("not an sqlite3 database")
	}

	d := &DB{
		f:        f,
		pageSize: h.PageSize(),
	}

	return d, nil
}

func (d *DB) Close() error {
	return d.f.Close()
}

func (d *DB) Info() (*Info, error) {
	t, err := d.readTable(1)
	if err != nil {
		return nil, fmt.Errorf("read schema: %w", err)
	}

	rows, err := t.rows()
	if err != nil {
		return nil, fmt.Errorf("count schema rows: %w", err)
	}

	inf := &Info{
		Tables: rows,
	}

	return inf, nil
}
