FROM alpine:3.18

# Add the testing repository
RUN echo "@testing http://dl-cdn.alpinelinux.org/alpine/edge/testing" >> /etc/apk/repositories

# Update the package list and install Zig
RUN apk add --no-cache zig@testing=0.11.0-r3

RUN echo "cd \${CODECRAFTERS_REPOSITORY_DIR} && zig build-exe ./app/main.zig" > /codecrafters-precompile.sh
RUN chmod +x /codecrafters-precompile.sh
