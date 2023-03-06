#!/bin/sh

if [ -d "./Assets/Plugins" ] 
then
  while true  
  do
    read -p "Plugins are already installed, do you wish to reinstall? Yes/[N]o " yn
        case $yn in
            [Yy]* ) break;;
            [Nn]* ) exit;;
            * ) echo "Please answer yes or no.";;
        esac
    echo 
    done
fi

download (){
  curl -L -o "package.zip" "$1"
  mkdir -p "./Assets/$2"
  unzip "package.zip" -d "./Assets/$2" > /dev/null
  rm -f "package.zip"
}

downloadNuget (){
  rm -fr "Assets/Plugins/$1"
  
  download "https://www.nuget.org/api/v2/package/$1/$2" "Plugins/$1"
  cd Assets/Plugins
  mv "$1/lib/$3" .
  rm -rf "$1"
  mkdir -p "$1/lib"
  mv "$3" "$1/lib/"
  cd ../..
}

downloadNuget Google.Protobuf 3.22.0 net45 && echo "installed protobuf"

downloadNuget System.Runtime.CompilerServices.Unsafe 4.5.2 netstandard2.0 && echo "installed system.unsafe"

downloadNuget Proyecto26.RestClient 2.6.2 net35
downloadNuget RSG.Promise 3.0.1 net35
echo "installed promise based rest"