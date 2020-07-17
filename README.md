# bundeswort

## What to install before using the app:
- Install dotnet
https://dotnet.microsoft.com/download/dotnet-core

- Install dotnet ef
dotnet tool install --global dotnet-ef

- Install docker
https://docs.docker.com/get-docker/

- Add **postgres** as a **localhost** in /etc/hosts

## Create/Update Database (should not be a step in the future):
dotnet ef database update

## How to run?
* docker-compose up -d
* dotnet watch run