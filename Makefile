THIS_FILE := $(lastword $(MAKEFILE_LIST))
export COMPOSE_BAKE=true
.PHONY: help build up start down destroy stop restart
# Docker Compose commands
build:
		docker-compose build $(c)
up:
		docker-compose up -d $(c)
start:
		docker-compose start $(c)
down:
		docker-compose down $(c)
destroy:
		docker-compose down -v $(c)
stop:
		docker-compose stop $(c)
restart:
		docker-compose stop $(c)
		docker-compose up -d $(c)

# Docker Bake
bake:
	docker buildx bake

bake-load:
	docker buildx bake --load

bake-push:
	docker buildx bake --push

# Docker Cleanup

clean-cache:
	docker buildx prune