use anyhow::{bail, Result};
use sqlite_starter_rust::{
    header::PageHeader, record::parse_record, schema::Schema, varint::parse_varint,
};
use std::convert::TryInto;
use std::fs::File;
use std::io::prelude::*;

/// parses variable size unsigned integer encoded as bytes to number.
fn parse_number(bytes: &[u8]) -> usize {
    let mut result: usize = 0;
    let num_bytes = bytes.len();
    for i in 0..num_bytes {
        let shift = (num_bytes - i - 1) * 8;
        result += (bytes[i] as usize) << shift;
    }
    result
}

fn main() -> Result<()> {
    // Parse arguments
    let args = std::env::args().collect::<Vec<_>>();
    match args.len() {
        0 | 1 => bail!("Missing <database path> and <command>"),
        2 => bail!("Missing <command>"),
        _ => {}
    }

    // Parse command and act accordingly
    let command = &args[2];
    match command.as_str() {
        ".dbinfo" => {
            let mut file = File::open(&args[1])?;
            let mut header = [0; 100];
            file.read_exact(&mut header)?;

            // The page size is stored at the 16th byte offset, using 2 bytes in little-endian order
            let page_size = u16::from_le_bytes([header[16], header[17]]);

            println!("database page size: {}", page_size);
        }
        _ => bail!("Missing or invalid command passed: {}", command),
    }

    Ok(())
}
