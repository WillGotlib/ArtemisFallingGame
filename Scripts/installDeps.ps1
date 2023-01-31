if (Test-Path -Path "./Assets/Plugins") {
    echo "Plugins are already installed"
    exit
}

$LIB_URL = "https://packages.grpc.io/archive/2022/04/67538122780f8a081c774b66884289335c290cbe-f15a2c1c-582b-4c51-acf2-ab6d711d2c59/csharp/grpc_unity_package.2.47.0-dev202204190851.zip"

Invoke-WebRequest -URI $LIB_URL -OutFile "unity-package.zip"
Expand-Archive "unity-package.zip" -DestinationPath "./Assets/"
rm "unity-package.zip" -force
echo "installed grpc packages"

$LIB_URL = "https://www.nuget.org/api/v2/package/System.Threading.Channels/7.0.0"

$folder="System.Threading.Channels"
Invoke-WebRequest -URI $LIB_URL -OutFile "threading-package.zip"
Expand-Archive "threading-package.zip" -DestinationPath "./Assets/Plugins/$folder"
rm "threading-package.zip" -force
cd Assets/Plugins
mv $folder/lib/net462 .
rm $folder -Recurse
mkdir $folder/lib -ea 0 > $null
mv net462 $folder/lib/
cd ../..
echo "installed channels"
