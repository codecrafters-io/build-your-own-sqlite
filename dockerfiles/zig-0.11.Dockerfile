FROM alpine:latest

RUN apk add --no-cache curl

ENV ZIG_VERSION=0.11.0

RUN cd /tmp \
  && wget https://ziglang.org/download/${ZIG_VERSION}/zig-linux-x86_64-${ZIG_VERSION}.tar.xz \
  && tar -Jxf zig-linux-x86_64-${ZIG_VERSION}.tar.xz \
  && mv zig-linux-x86_64-${ZIG_VERSION}/zig /usr/bin \
  && mv zig-linux-x86_64-${ZIG_VERSION}/lib /usr/bin \
  && rm -rf zig-linux-x86_64-${ZIG_VERSION} \
  && rm zig-linux-x86_64-${ZIG_VERSION}.tar.xz

RUN zig version

RUN echo "cd \${CODECRAFTERS_SUBMISSION_DIR} && zig build-exe ./app/main.zig" > /codecrafters-precompile.sh
RUN chmod +x /codecrafters-precompile.sh