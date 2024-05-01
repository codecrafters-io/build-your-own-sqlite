The entry point for your SQLite implementation is in `app/main.ts`.

Study and uncomment the relevant code: 

```typescript
// Uncomment this to pass the first stage
const pageSize = new DataView(buffer.buffer, 0, buffer.byteLength).getUint16(16);
console.log(`database page size: ${pageSize}`);
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
