const databaseFilePath: string = Deno.args[0];
const command: string = Deno.args[1];

if (command === ".dbinfo") {
    const databaseFileHandler = await Deno.open(databaseFilePath, { read: true });
    const buffer = new Uint8Array(100);
    await databaseFileHandler.read(buffer);

    // You can use print statements as follows for debugging, they'll be visible when running tests.
    console.log("Logs from your program will appear here!");

    // Uncomment this to pass the first stage    
    // const pageSize = new DataView(buffer.buffer, 0, buffer.byteLength).getUint16(16);
    // console.log(`database page size: ${pageSize}`);

    databaseFileHandler.close();
} else {
    throw new Error(`Unknown command ${command}`);
}
