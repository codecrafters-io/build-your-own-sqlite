package dbfile

import "io"

/*
B-Tree page layout

	// Offset Size Description
	pageHeader   []byte // 8 or 12 bytes
	cellPointers []uint16
	// unallocated space
	cellsContent []byte

B-Tree page header layout

	flags               uint8
	firstFreeBlock      uint16
	cells               uint16
	cellContent         uint16
	fragmentedFreeBytes uint8
	rightmostPointer    uint32 // appears only on interior b-tree pages

*/

type (
	BTreePage struct {
		page
		headerSize int
	}

	PageType byte
)

const (
	InteriorIndex PageType = 0x2
	InteriorTable PageType = 0x5
	LeafIndex     PageType = 0xa
	LeafTable     PageType = 0xd
)

func ReadBTreePage(r io.ReaderAt, off, size int64) (BTreePage, error) {
	p := BTreePage{
		page: makePage(size),
	}

	_, err := r.ReadAt(p.page, off)
	if err != nil {
		return BTreePage{}, err
	}

	if typ := PageType(p.page.u8(0)); typ == LeafIndex || typ == LeafTable {
		p.headerSize = 8
	} else {
		p.headerSize = 12
	}

	return p, nil
}

func (p BTreePage) PageType() PageType { return PageType(p.u8(0)) }

func (p BTreePage) IsLeaf() bool {
	t := p.PageType()

	return t == LeafIndex || t == LeafTable
}

func (p BTreePage) Cells() int {
	return int(p.u16(3))
}

func (p BTreePage) CellOffset(n int) int {
	return int(p.u16(p.headerSize + n*2))
}
