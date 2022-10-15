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
	var n int

	err := t.walk(t.root, func(p dbfile.BTreePage) error {
		if !p.IsLeaf() {
			return nil
		}

		n += p.Cells()

		return nil
	})
	if err != nil {
		return 0, fmt.Errorf("walk b-tree: %w", err)
	}

	return n, nil
}

func (t *Table) walk(p dbfile.BTreePage, f func(p dbfile.BTreePage) error) error {
	if p.IsLeaf() {
		return f(p)
	}

	panic("implement walking interior pages here")

	for i := 0; i < p.Cells(); i++ {
		// c := p.Cell(i)
	}
}
