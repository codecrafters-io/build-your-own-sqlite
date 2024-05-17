The entry point for your SQLite implementation is in `app/main.js`.

Study and uncomment the relevant code: 

```javascript
// Uncomment this to pass the first stage
const pageSize = buffer.readUInt16BE(16); // page size is 2 bytes starting at offset 16
console.log(`database page size: ${pageSize}`);
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
