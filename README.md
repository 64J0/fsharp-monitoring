# F# API with Prometheus, Grafana and Alert Manager

:warning: Work in progress!

This project consists in a simple F# REST API to exemplify how to work with Prometheus and Grafana. To do the connection between this .NET project and the Prometheus gathering, we'll use the [prometheus-net](https://github.com/prometheus-net/prometheus-net) project.

Along with this code, I'm also writing an article that later will be posted in my Medium page to explain the core concepts of each tool used, with more details regarding the API built.
## How to run it?

Make sure you have the following tools installed:

* .NET SDK version 6.0.200

```bash
# use this command to check your installed versions
$ dotnet --list-sdks
```

Next step is to install the required dependencies, using the following commands:

```bash
# restore some nuget tools
$ dotnet tool restore

# make sure you can build the project
$ dotnet build

# start the server
$ dotnet run
# watch mode for development
# dotnet run --watch
```

## How to test it

```bash
# test endpoint
curl localhost:8085/test

# trying the POST endpoint
curl -X POST \
    -H "Accept: application/json" \
    -d '{"id":"1", "crimesPerCapta":"0.01"}' \
    localhost:8085/api/prediction
```

You can later see the metrics by visiting `localhost:8085/metrics` in your browser.

## Run with Docker-compose

```bash
# docker image standalone
docker build -t prometheus-api:v1 .
docker run -d -e HOST="0.0.0.0" -p 8085:8085 prometheus-api:v1

# docker-compose
docker-compose up -d
```

When this project is running you can visit `localhost:9090` and start grabbing the metrics for our project from the Prometheus interface. The docker-compose configuration for the Prometheus service was mainly inspired by [this reference](https://github.com/vegasbrianc/prometheus/blob/master/docker-compose.yml).