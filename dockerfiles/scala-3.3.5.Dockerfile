# syntax=docker/dockerfile:1.7-labs
FROM maven:3.9.9-eclipse-temurin-23-alpine

RUN apk add --no-cache bash wget tar && \
    wget -O sbt.tgz https://github.com/sbt/sbt/releases/download/v1.10.11/sbt-1.10.11.tgz && \
    tar -xzf sbt.tgz -C /usr/local && \
    ln -s /usr/local/sbt/bin/sbt /usr/local/bin/sbt && \
    rm sbt.tgz

# Ensures the container is re-built if dependency files change
ENV CODECRAFTERS_DEPENDENCY_FILE_PATHS="build.sbt"

WORKDIR /app

# .git & README.md are unique per-repository. We ignore them on first copy to prevent cache misses
COPY --exclude=.git --exclude=README.md . /app

# Install language-specific dependencies
RUN .codecrafters/compile.sh
