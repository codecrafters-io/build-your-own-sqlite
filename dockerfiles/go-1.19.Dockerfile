FROM golang:1.19-alpine

COPY go.mod /app/go.mod
COPY go.sum /app/go.sum

WORKDIR /app

RUN go mod download

ENV CODECRAFTERS_DEPENDENCY_FILE_PATHS="go.mod,go.sum"
