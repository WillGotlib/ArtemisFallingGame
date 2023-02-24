if (Test-Path -Path "./Assets/Plugins") {
    echo "Plugins are already installed"
    exit
}

$LIB_URL = "https://www.nuget.org/api/v2/package/Google.Protobuf/3.22.0"

$folder="Google.Protobuf"
Invoke-WebRequest -URI $LIB_URL -OutFile "protobuf-package.zip"
Expand-Archive "protobuf-package.zip" -DestinationPath "./Assets/Plugins/$folder"
rm "protobuf-package.zip" -force
cd Assets/Plugins
mv $folder/lib/netstandard2.0 .
rm $folder -Recurse
mkdir $folder/lib -ea 0 > $null
mv netstandard2.0 $folder/lib/
cd ../..
echo "installed protobuf"