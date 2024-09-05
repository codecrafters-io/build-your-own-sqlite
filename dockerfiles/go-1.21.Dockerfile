FROM golang:1.19-alpine

COPY go.mod /app/go.mod
COPY go.sum /app/go.sum

WORKDIR /app

# Starting from Go 1.20, the go standard library is no loger compiled
# setting the GODEBUG environment to "installgoroot=all" restores the old behavior
RUN GODEBUG="installgoroot=all" go install std

RUN go mod download

ENV CODECRAFTERS_DEPENDENCY_FILE_PATHS="go.mod,go.sum"
