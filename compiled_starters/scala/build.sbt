ThisBuild / scalaVersion := "3.3.5"
ThisBuild / version := "0.1.0-SNAPSHOT"
ThisBuild / organization := "com.CodeCrafters"
ThisBuild / organizationName := "CodeCrafters"

assembly / assemblyJarName := "sqlite"

lazy val root = (project in file("."))
  .settings(
    name := "codecrafter-sqlite",
    // List your dependencies here
    libraryDependencies ++= Seq(
    ),
  )