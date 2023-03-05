if (Test-Path -Path "./Assets/Plugins") {
    echo "Plugins are already installed"
    exit
}

function Download-NuGet{
    param (
        $Package,
        $PackageVersion,
        $CVersion
    )

    $LIB_URL = "https://www.nuget.org/api/v2/package/$Package/$PackageVersion"

    Invoke-WebRequest -URI $LIB_URL -OutFile "nuget-package.zip"
    Expand-Archive "nuget-package.zip" -DestinationPath "./Assets/Plugins/$Package"
    rm "nuget-package.zip" -force
    cd Assets/Plugins
    mv $Package/lib/$CVersion .
    rm $Package -Recurse
    mkdir $Package/lib -ea 0 > $null
    mv $CVersion $Package/lib/
    cd ../..
}

Download-NuGet Google.Protobuf 3.22.0 net45
echo "installed protobuf"

Download-NuGet System.Runtime.CompilerServices.Unsafe 4.5.2 netstandard2.0
echo "installed system.unsafe"

Download-NuGet System.Threading.Channels 7.0.0 net462
echo "installed channels"