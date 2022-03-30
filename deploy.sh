#! /bin/sh

# base images
for s in \
	eassbhhtgu/networktest-db:latest \
	eassbhhtgu/networktest:latest
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

# docker deploy
docker stack deploy --compose-file ./docker-compose.yml networktest
