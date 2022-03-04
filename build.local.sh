#! /bin/sh

# base image
docker pull mariadb:latest

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

# docker deploy
docker stack deploy --compose-file ./docker-compose.local.yml networktest
