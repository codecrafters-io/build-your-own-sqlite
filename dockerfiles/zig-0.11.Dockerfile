FROM alpine:edge

# Add the testing repository
RUN echo "@testing http://dl-cdn.alpinelinux.org/alpine/edge/testing" >> /etc/apk/repositories

# Update the package list and install Zig
RUN apk update && apk add zig@testing=0.11.0-r0
