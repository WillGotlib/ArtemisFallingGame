#!/bin/sh

if [ -d "./Assets/Plugins" ] 
then
    echo "Plugins are already installed"
    exit
fi

download (){
  curl -L -o "package.zip" $1
  unzip "package.zip" -d "./Assets/$2" > /dev/null
  rm -f "package.zip"
}

downloadNuget (){
  download "https://www.nuget.org/api/v2/package/$1/$2" "Plugins/$1
"
  cd Assets/Plugins
  mv "$1/lib/net462" .
  rm -rf "$1"
  mkdir -p "$1/lib"
  mv "$3" "$1/lib/"
  cd ../..
  echo "installed channels"
}

downloadNuget Google.Protobuf 3.22.0 net45
echo "installed protobuf"

downloadNuget System.Runtime.CompilerServices.Unsafe 4.5.2 netstandard2.0
echo "installed system.unsafe"