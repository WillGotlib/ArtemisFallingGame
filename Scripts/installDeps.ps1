if (Test-Path -Path "./Assets/Plugins") {
    $confirmation = Read-Host "Plugins are already installed, do you wish to reinstall? y/[n] "
    if ($confirmation -ne 'y') {
        exit
    }
}

function Download-NuGet{
    param (
        $Package,
        $PackageVersion,
        $CVersion
    )

    $LIB_URL = "https://www.nuget.org/api/v2/package/$Package/$PackageVersion"

    $path = "./Assets/Plugins/$Package"
    if (Test-Path $path) {
        Remove-Item $path -Recurse
    }
    
    Invoke-WebRequest -URI $LIB_URL -OutFile "nuget-package.zip"
    Expand-Archive "nuget-package.zip" -DestinationPath $path
    Remove-Item "nuget-package.zip" -Force
    cd Assets/Plugins
    Move-Item -Path $Package/lib/$CVersion -Destination .
    Remove-Item $Package -Recurse
    mkdir $Package/lib -ea 0 > $null
    Move-Item -Path $CVersion -Destination $Package/lib/
    cd ../..
}

Download-NuGet Google.Protobuf 3.22.0 net45
echo "installed protobuf"

Download-NuGet System.Runtime.CompilerServices.Unsafe 4.5.2 netstandard2.0
echo "installed system.unsafe"

Download-NuGet Proyecto26.RestClient 2.6.2 net35
Download-NuGet RSG.Promise 3.0.1 net35
echo "installed promise based rest"