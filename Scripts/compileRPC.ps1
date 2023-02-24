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

$location = "GameJamJan21/Assets/Scripts/Analytics"
mkdir $location -ea 0 > $null
& $protoc --csharp_out=$location Protobuf/*.proto
if ($?) {
    echo "built unity protos"
}