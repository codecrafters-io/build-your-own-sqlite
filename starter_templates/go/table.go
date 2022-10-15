package sqlite

import (
	"fmt"
	"github/com/codecrafters-io/sqlite-starter-go/dbfile"
)

type (
	Table struct {
		root dbfile.BTreePage
	}
)

func (d *DB) readTable(page int) (*Table, error) {
	page-- // pages are numbered from 1

	off := d.pageSize * int64(page)
	size := d.pageSize

	if page == 0 {
		off += 100 // skip db header
		size -= 100
	}

	p, err := dbfile.ReadBTreePage(d.f, off, size)
	if err != nil {
		return nil, fmt.Errorf("read page: %w", err)
	}

	t := &Table{
		root: p,
	}

	return t, nil
}

func (t *Table) rows() (int, error) {
	return t.root.Cells(), nil
}
