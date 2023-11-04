FROM alpine:3.18

# Add the testing repository
RUN echo "@testing http://dl-cdn.alpinelinux.org/alpine/edge/testing" >> /etc/apk/repositories

# Update the package list and install Zig
RUN apk update && apk add zig@testing=0.11.0-r1

RUN echo "cd \${CODECRAFTERS_SUBMISSION_DIR} && zig build-exe ./app/main.zig" > /codecrafters-precompile.sh
RUN chmod +x /codecrafters-precompile.sh