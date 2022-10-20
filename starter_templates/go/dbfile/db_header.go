package dbfile

import (
	"bytes"
	"fmt"
	"io"
)

const magic = "SQLite format 3\000"

func init() {
	if len(magic) != 16 {
		panic("bad magic")
	}
}

type DBHeader struct {
	page
}

func ReadDBHeader(r io.ReaderAt) (DBHeader, error) {
	h := DBHeader{
		page: makePage(100),
	}

	_, err := r.ReadAt(h.page, 0)
	if err != nil {
		return DBHeader{}, err
	}

	if !bytes.HasPrefix(h.page, []byte(magic)) {
		return DBHeader{}, fmt.Errorf("not an sqlite3 database")
	}

	return h, nil
}

func (p DBHeader) PageSize() int64 {
	return int64(p.i16(16))
}
