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

folder="Google.Protobuf"
download "https://www.nuget.org/api/v2/package/Google.Protobuf/3.22.0" "Plugins/$folder"
cd Assets/Plugins
mv $folder/lib/netstandard2.0 .
rm -rf $folder
mkdir -p $folder/lib
mv netstandard2.0 $folder/lib/
cd ../..
echo "installed protobuf"
