if (Test-Path -Path "./Assets/Plugins") {
    echo "Plugins are already installed"
    exit
}

$LIB_URL = "https://packages.grpc.io/archive/2022/04/67538122780f8a081c774b66884289335c290cbe-f15a2c1c-582b-4c51-acf2-ab6d711d2c59/csharp/grpc_unity_package.2.47.0-dev202204190851.zip"

Invoke-WebRequest -URI $LIB_URL -OutFile "unity-package.zip"
Expand-Archive "unity-package.zip" -DestinationPath "./Assets/"
rm "unity-package.zip" -force
echo "installed grpc packages"
