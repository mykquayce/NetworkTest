#! /bin/sh

# base images
for s in mcr.microsoft.com/dotnet/sdk:6.0 mcr.microsoft.com/dotnet/runtime:6.0 mariadb:latest
do
	docker pull $s
done

# secrets
for s in networktest_mariadb_password
do
  docker secret ls | tail --line +2 | grep $s >/dev/null

  if [ $? -ne 0 ]; then
    openssl rand -base64 201 | sed 's/[^0-9A-Za-z]//g' | sed -z 's/\n//g' | docker secret create $s -
  fi
done

# build database image
docker build --tag eassbhhtgu/networktest-db:latest ./Database
docker push eassbhhtgu/networktest-db:latest

# build worker image
docker build --tag eassbhhtgu/networktest:latest ./Worker
docker push eassbhhtgu/networktest:latest

# docker deploy
docker stack deploy --compose-file ./docker-compose.yml networktest
