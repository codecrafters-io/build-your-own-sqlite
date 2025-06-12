The entry point for your SQLite implementation is in `src/sqlite/core.clj`.

Study and uncomment the relevant code: 

```clojure
;; Uncomment this block to pass the first stage
(let [command (second args)]
  (case command
    ".dbinfo"
    (let [db-file-path (first args)
          contents (lazy-byte-seq db-file-path)
          page-size (bytes-to-int (take 2 (drop 16 contents)))]
      (println (str "database page size: " page-size)))))
```

Push your changes to pass the first stage:

```
git add .
git commit -m "pass 1st stage" # any msg
git push origin master
```
