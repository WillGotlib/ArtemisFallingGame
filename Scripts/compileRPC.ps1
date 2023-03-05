$OperatingSystem = "windows"
$Version = "x64"

$OS = $OperatingSystem +"_"+$Version
$GRPC_PATH ="./Grpc-Tools"
$GRPC_URL = "https://www.nuget.org/api/v2/package/Grpc.Tools/"
if (-not(Test-Path -Path $GRPC_PATH)) {
    $temp_dir = $GRPC_PATH+"/tmp" 
    mkdir $temp_dir -ea 0 > $null
    cd $temp_dir
    Invoke-WebRequest -URI $GRPC_URL -OutFile "tmp.zip"
    Expand-Archive "tmp.zip" -DestinationPath "."

    cd ..
    Get-ChildItem -Path "tmp/tools/$OS" -Recurse | Move-Item -Destination "."
    rm "tmp" -r -force
    cd ..
}

$protoc = $GRPC_PATH+"/protoc.exe"

if (Get-Command "go" -errorAction SilentlyContinue){
    if (-not(Get-Command "protoc-gen-go" -errorAction SilentlyContinue)){
        go install google.golang.org/protobuf/cmd/protoc-gen-go@latest
    }
    if (-not(Get-Command "protoc-gen-go-grpc" -errorAction SilentlyContinue)){
        go install google.golang.org/grpc/cmd/protoc-gen-go-grpc@latest
    }
    & $protoc --go-grpc_out=Server/ --go_out=Server/ Protobuf/Online/*.proto
    if ($?) {
        echo "built go protos"
    }
}

$csharpPlugin = $GRPC_PATH+"/grpc_csharp_plugin.exe"
$location = "GameJamJan21/Assets/Scripts/Online/Generated"
mkdir $location -ea 0 > $null
& $protoc --csharp_out=$location --grpc_out=$location --plugin=protoc-gen-grpc=$csharpPlugin Protobuf/Online/*.proto
if ($?) {
    echo "built unity protos"
}

$location = "GameJamJan21/Assets/Scripts/Analytics"
mkdir $location -ea 0 > $null
& $protoc --csharp_out=$location Protobuf/Analytics/*.proto
if ($?) {
    echo "built Analytics protos"
}