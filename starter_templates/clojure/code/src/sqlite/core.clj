(ns sqlite.core
  (:gen-class)
  (:require [clojure.java.io :as io]))

(defn lazy-byte-seq [filepath]
  (let [is (io/input-stream filepath)
        step (fn step [input-stream]
               (lazy-seq (let [byte-val (.read input-stream)]
                           (when (not= byte-val -1)
                             (cons byte-val (step input-stream))))))]
    (step is)))

(defn bytes-to-int [bytes]
  (reduce (fn [acc b] (+ (* acc 256) b))
          0
          bytes))

(defn -main [& args]
;; You can use print statements as follows for debugging, they'll be
;; visible when running tests.
  (println "Logs from your program will appear here!")
;; Uncomment this block to pass the first stage
;; (let [command (second args)]
;;   (case command
;;     ".dbinfo"
;;     (let [db-file-path (first args)
;;           contents (lazy-byte-seq db-file-path)
;;           page-size (bytes-to-int (take 2 (drop 16 contents)))]
;;       (println (str "database page size: " page-size)))))
)
