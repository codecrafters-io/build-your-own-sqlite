(ns build
  (:gen-class)
  (:require [clojure.tools.build.api :as b]))

(def lib 'io.codecrafters.http-server)
(def class-dir "/tmp/codecrafters-build-http-server-clojure/classes")
(def basis (b/create-basis {:project "deps.edn"}))
(def uber-file "/tmp/codecrafters-build-http-server-clojure/target.jar")

(defn clean
  [_]
  (b/delete {:path "/tmp/codecrafters-build-http-server-clojure"}))

(defn uber
  [_]
  (clean nil)
  (b/copy-dir {:src-dirs ["src"], :target-dir class-dir})
  (b/compile-clj
    {:basis basis, :ns-compile '[http-server.core], :class-dir class-dir})
  (b/uber {:class-dir class-dir,
           :uber-file uber-file,
           :basis basis,
           :main 'http-server.core}))
