# syntax=docker/dockerfile:1
FROM --platform=linux/amd64 namely/protoc as PROTOCOMPILER
WORKDIR /compile

COPY Protobuf/ .
RUN protoc --go-grpc_out=./ --go_out=./ ./*.proto && echo "built go protos"


FROM golang:alpine as BUILDER
WORKDIR /build

COPY Server/go.mod Server/go.sum ./
RUN go mod download

COPY Server/ ./
COPY --from=PROTOCOMPILER /compile/proto proto/

RUN CGO_ENABLED=0 go build -ldflags="-w -s" -o server


FROM scratch
WORKDIR /app

COPY --from=BUILDER /build/server .

EXPOSE 37892
CMD ["/app/server"]