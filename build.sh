#! /bin/bash

for s in mcr.microsoft.com/dotnet/sdk:6.0 mcr.microsoft.com/dotnet/runtime:6.0 mariadb:latest
do
	docker pull $s
done

docker build --tag eassbhhtgu/networktest:latest .

docker push eassbhhtgu/networktest:latest
