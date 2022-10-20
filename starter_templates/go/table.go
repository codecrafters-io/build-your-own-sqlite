package sqlite

import (
	"fmt"
	"github/com/codecrafters-io/sqlite-starter-go/dbfile"
)

type (
	Table struct {
		rootPage dbfile.BTreePage
	}
)

func (d *DB) readTable(pageNumber int) (*Table, error) {
	offset := d.pageSize * int64(pageNumber-1) // pages are numbered from 1
	size := d.pageSize

	if pageNumber == 1 { // first page
		offset += 100 // skip db header
		size -= 100
	}

	page, err := dbfile.ReadBTreePage(d.f, offset, size)
	if err != nil {
		return nil, fmt.Errorf("read page: %w", err)
	}

	t := &Table{
		rootPage: page,
	}

	return t, nil
}

func (t *Table) rows() (int, error) {
	var n int

	err := t.walk(t.rootPage, func(page dbfile.BTreePage) error {
		if !page.IsLeaf() {
			return nil
		}

		n += page.Cells()

		return nil
	})
	if err != nil {
		return 0, fmt.Errorf("walk b-tree: %w", err)
	}

	return n, nil
}

func (t *Table) walk(page dbfile.BTreePage, f func(page dbfile.BTreePage) error) error {
	if page.IsLeaf() {
		return f(page)
	}

	panic("implement walking interior pages here")
}
