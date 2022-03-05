#! /bin/sh

# base image
docker pull mariadb:latest


# build database image
docker build --tag eassbhhtgu/networktest-db:latest ./Database
docker push eassbhhtgu/networktest-db:latest

# docker deploy
docker stack deploy --compose-file ./docker-compose.local.yml networktest
