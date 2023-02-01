FROM namely/protoc as PROTOCOMPILER
WORKDIR /compile

COPY Protobuf/ .
RUN protoc --go-grpc_out=./ --go_out=./ ./*.proto && echo "built go protos"

FROM --platform=${BUILDPLATFORM:-linux/amd64} golang:alpine as BUILDER
WORKDIR /build

ARG TARGETPLATFORM
ARG BUILDPLATFORM
ARG TARGETOS
ARG TARGETARCH

COPY Server/ ./
COPY --from=PROTOCOMPILER /compile/proto proto/

RUN CGO_ENABLED=0 GOOS=${TARGETOS} GOARCH=${TARGETARCH} go build -ldflags="-w -s" -o server

FROM gruebel/upx as COMPRESSOR
WORKDIR /compress

COPY --from=BUILDER /build/server ./server.org

RUN upx --best --lzma -o server server.org

FROM --platform=${TARGETPLATFORM:-linux/amd64} scratch
WORKDIR /app

COPY --from=COMPRESSOR /compress/server ./

EXPOSE 37892
CMD ["/app/server"]