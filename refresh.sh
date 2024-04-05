#! /bin/sh

docker pull mariadb:latest
docker pull mcr.microsoft.com/dotnet/runtime:8.0
docker pull mcr.microsoft.com/dotnet/sdk:8.0
docker pull eassbhhtgu/networktest:latest
docker pull eassbhhtgu/networktest-db:latest



base1=$(docker image inspect --format '{{.Created}}' mcr.microsoft.com/dotnet/runtime:8.0)
base2=$(docker image inspect --format '{{.Created}}' mcr.microsoft.com/dotnet/sdk:8.0)
base3=$(docker image inspect --format '{{.Created}}' mariadb:latest)
image1=$(docker image inspect --format '{{.Created}}' eassbhhtgu/networktest:latest)
image2=$(docker image inspect --format '{{.Created}}' eassbhhtgu/networktest-db:latest)



if [[ $base1 > $image1 ]] || [[ $base2 > $image1 ]] || [[ $base3 > $image2 ]]; then

	# build database image
	docker build --tag eassbhhtgu/networktest-db:latest ./Database
	docker push eassbhhtgu/networktest-db:latest

	# build worker image
	docker build \
		--secret id=ca_crt,src=${USERPROFILE}/.aspnet/https/ca.crt \
		--tag eassbhhtgu/networktest:latest \
		./Worker
	docker push eassbhhtgu/networktest:latest
fi
