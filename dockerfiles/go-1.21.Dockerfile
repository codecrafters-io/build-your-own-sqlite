# syntax=docker/dockerfile:1.7-labs
FROM golang:1.19-alpine


WORKDIR /app
# .git & README.md are unique per-repository. We ignore them on first copy to prevent cache misses
COPY --exclude=.git --exclude=README.md . /app

RUN go mod download

# Starting from Go 1.20, the go standard library is no loger compiled
# setting the GODEBUG environment to "installgoroot=all" restores the old behavior
RUN GODEBUG="installgoroot=all" go install std

# Even though modules are downloaded, building them could take a while.
# Let's run go get on each module so that they're built ahead of time.
# Ref: https://github.com/montanaflynn/golang-docker-cache
RUN ash -c "set -exo pipefail; go mod graph | awk '{if (\$1 !~ \"@\") {print \$2}}' | xargs -r go get"

ENV CODECRAFTERS_DEPENDENCY_FILE_PATHS="go.mod,go.sum"

# Once the heave steps are done, we can copy all files back
COPY . /app
