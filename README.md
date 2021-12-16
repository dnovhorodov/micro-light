## Lightweight Microservices with F# and ZeroMQ

This is a sample solution for an [article](https://danylomeister.blog/2021/12/23/lightweight-microservices-with-fsharp-and-zeromq/) from [F# Advent 2021](https://sergeytihon.com/2021/10/18/f-advent-calendar-2021/). Solution consists of two projects: pub and sub which represents publisher and subscriber accordingly. Publisher randomly sends domain events: correct and incorrect, subscriber listens to it and validates.

------

## Get Started

- For Windows install latest  [Docker Desktop](https://www.docker.com/products/docker-desktop), for Linux install according to your distro. Follow the [official manual](https://docs.docker.com/engine/install/ubuntu/).

- Clone this repository 

- Run following command in the root folder:
  ```sh
  docker compose up -d
  ```

- Check that publisher and subscriber services communicate by sending events:
  ```sh
  docker compose logs -f pub
  docker compose logs -f sub
  ```

You can also build it and run locally:
  ```sh
  dotnet build
  dotnet run --project ./src/pub/pub.fsproj
  dotnet run --project ./src/sub/sub.fsproj
  ```
