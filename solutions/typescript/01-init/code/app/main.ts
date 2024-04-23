const databaseFilePath: string = Deno.args[0];
const command: string = Deno.args[1];

if (command === ".dbinfo") {
    const databaseFileHandler = await Deno.open(databaseFilePath, { read: true });
    const buffer = new Uint8Array(100);
    await databaseFileHandler.read(buffer);

    const pageSize = new DataView(buffer.buffer, 0, buffer.byteLength).getUint16(16);
    console.log(`database page size: ${pageSize}`);

    databaseFileHandler.close();
} else {
    throw new Error(`Unknown command ${command}`);
}
