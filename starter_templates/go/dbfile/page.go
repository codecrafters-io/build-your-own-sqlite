package dbfile

import "encoding/binary"

type page []byte

func makePage(size int64) page {
	return make(page, size)
}

func (p page) u8(offset int) uint8 {
	return p[offset]
}

func (p page) i8(offset int) int8 {
	return int8(p[offset])
}

func (p page) u16(offset int) uint16 {
	return binary.BigEndian.Uint16(p[offset:])
}

func (p page) i16(offset int) int16 {
	return int16(binary.BigEndian.Uint16(p[offset:]))
}
