package dbfile

import (
	"bytes"
	"io"
)

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

	return h, nil
}

func (p DBHeader) Magic() string {
	null := bytes.IndexByte(p.page, '\000')
	if null == -1 {
		return ""
	}

	return string(p.page[:null])
}

func (p DBHeader) PageSize() int64 {
	return int64(p.i16(16))
}
