import { open } from "fs/promises";

const databaseFilePath = process.argv[2];
const command = process.argv[3];

if (command === ".dbinfo") {
  const databaseFileHandler = await open(databaseFilePath, "r");

  const { buffer } = await databaseFileHandler.read({
    length: 100,
    position: 0,
    buffer: Buffer.alloc(100),
  });

  const pageSize = buffer.readUInt16BE(16); // page size is 2 bytes starting at offset 16
  console.log(`database page size: ${pageSize}`);
} else {
  throw `Unknown command ${command}`;
}
