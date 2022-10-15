package main

import (
	"fmt"
	"github/com/codecrafters-io/sqlite-starter-go"
	"os"
	"path/filepath"
)

// Usage: your_sqlite3.sh sample.db .dbinfo
func main() {
	if len(os.Args) < 3 {
		fmt.Fprintf(os.Stderr, "usage: %v <sample.db> <command>\n", filepath.Base(os.Args[0]))
		os.Exit(1)
	}

	dbPath := os.Args[1]
	command := os.Args[2]

	var err error

	switch command {
	case ".dbinfo":
		err = dbinfo(dbPath)
	default:
		err = fmt.Errorf("unknown command: %v", command)
	}

	if err != nil {
		fmt.Fprintf(os.Stderr, "error: %v\n", err)
		os.Exit(1)
	}
}

func dbinfo(dbpath string) (err error) {
	db, err := sqlite.Open(dbpath)
	if err != nil {
		return fmt.Errorf("open db: %w", err)
	}

	defer func() {
		e := db.Close()
		if err == nil && e != nil {
			err = fmt.Errorf("close db: %w", e)
		}
	}()

	info, err := db.Info()
	if err != nil {
		return fmt.Errorf("get db info: %w", err)
	}

	fmt.Printf("number of tables: %v\n", info.Tables)

	return nil
}
