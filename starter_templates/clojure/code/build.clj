(ns build
  (:gen-class)
  (:require [clojure.tools.build.api :as b]))

(def lib 'io.codecrafters.sqlite)
(def class-dir "/tmp/codecrafters-build-sqlite-clojure/classes")
(def basis (b/create-basis {:project "deps.edn"}))
(def uber-file "/tmp/codecrafters-build-sqlite-clojure/target.jar")

(defn clean [_]
  (b/delete {:path "/tmp/codecrafters-build-sqlite-clojure"}))

(defn uber [_]
  (clean nil)
  (b/copy-dir {:src-dirs ["src"] :target-dir class-dir})
  (b/compile-clj {:basis basis
                  :ns-compile '[sqlite.core]
                  :class-dir class-dir})
  (b/uber {:class-dir class-dir
           :uber-file uber-file
           :basis basis
           :main 'sqlite.core}))
