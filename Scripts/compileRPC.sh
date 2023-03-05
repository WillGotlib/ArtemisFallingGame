#!/bin/sh

# linux macosx windows
if [ "$(uname)" = "Darwin" ]; then
  OperatingSystem="macosx"
else
  OperatingSystem="linux"
fi 

Version="x64" # arm64 (only on linux) x64 x86

OS="${OperatingSystem}_${Version}"
echo installing tools for $OS

GRPC_PATH="./Grpc-Tools"
GRPC_URL="https://www.nuget.org/api/v2/package/Grpc.Tools/"
if [ ! -d $GRPC_PATH ] 
then
    tempDir="$GRPC_PATH/tmp"
    mkdir -p $tempDir
    cd $tempDir
    curl -L -o tmp.zip $GRPC_URL
    unzip tmp.zip > /dev/null

    ls tools
    cd ..
    mv -v tmp/tools/$OS/* .
    chmod +x *
    rm -rf tmp
    cd ..
fi

protoc="${GRPC_PATH}/protoc"
if command -v go &> /dev/null
then
    if ! command -v protoc-gen-go &> /dev/null
    then
        go install google.golang.org/protobuf/cmd/protoc-gen-go@latest
    fi

    eval $protoc --go_out=Server/ Protobuf/Online/*.proto && echo "built go protos"
fi

eval $protoc --csharp_out=Client/Assets/Scripts/Online/Generated Protobuf/Online/*.proto && echo "built unity protos"
eval $protoc --csharp_out=GameJamJan21/Assets/Scripts/Analytics Protobuf/Analytics/*.proto && echo "built analytics protos"
