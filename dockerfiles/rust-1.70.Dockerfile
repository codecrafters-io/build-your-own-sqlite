# syntax=docker/dockerfile:1.7-labs
FROM rust:1.70-buster


RUN mkdir /app/src
RUN echo 'fn main() { println!("Hello World!"); }' > /app/src/main.rs

WORKDIR /app
# .git & README.md are unique per-repository. We ignore them on first copy to prevent cache misses
COPY --exclude=.git --exclude=README.md . /app
RUN cargo build --release --target-dir=/tmp/codecrafters-sqlite-target

RUN rm /tmp/codecrafters-sqlite-target/release/sqlite-starter-rust
RUN rm /tmp/codecrafters-sqlite-target/release/sqlite-starter-rust.d

RUN find /tmp/codecrafters-sqlite-target/release -type f -maxdepth 1 -delete
RUN rm -f /tmp/codecrafters-sqlite-target/release/deps/*sqlite_starter_rust*
RUN rm -f /tmp/codecrafters-sqlite-target/release/deps/sqlite_starter_rust*
RUN rm -f /tmp/codecrafters-sqlite-target/release/.fingerprint/*sqlite_starter_rust*
RUN rm -f /tmp/codecrafters-sqlite-target/release/.fingerprint/sqlite_starter_rust*

RUN rm -rf /app/src

RUN echo "cd \${CODECRAFTERS_SUBMISSION_DIR} && cargo build --release --target-dir=/tmp/codecrafters-sqlite-target --manifest-path Cargo.toml" > /codecrafters-precompile.sh
RUN chmod +x /codecrafters-precompile.sh

ENV CODECRAFTERS_DEPENDENCY_FILE_PATHS="Cargo.toml,Cargo.lock"


# Once the heave steps are done, we can copy all files back
COPY . /app
