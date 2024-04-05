#! /bin/sh

# base images
for s in \
	mariadb:latest \
	mcr.microsoft.com/dotnet/runtime:8.0 \
	mcr.microsoft.com/dotnet/sdk:8.0
do
	docker pull $s
done

# build database image
docker build --tag eassbhhtgu/networktest-db:latest ./Database
docker push eassbhhtgu/networktest-db:latest

# build worker image
docker build \
	--secret id=ca_crt,src=${USERPROFILE}/.aspnet/https/ca.crt \
	--tag eassbhhtgu/networktest:latest \
	./Worker
docker push eassbhhtgu/networktest:latest
