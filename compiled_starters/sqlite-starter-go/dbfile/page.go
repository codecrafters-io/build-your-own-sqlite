package dbfile

import "encoding/binary"

type page []byte

func makePage(size int64) page {
	return make(page, size)
}

func (p page) u8(off int) uint8 {
	return p[off]
}

func (p page) i8(off int) int8 {
	return int8(p[off])
}

func (p page) u16(off int) uint16 {
	return binary.BigEndian.Uint16(p[off:])
}

func (p page) i16(off int) int16 {
	return int16(binary.BigEndian.Uint16(p[off:]))
}
