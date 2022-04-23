# F# API with Prometheus and Grafana

This project consists in a simple F# REST API to exemplify how to work with Prometheus and Grafana. To do the connection between this .NET project and the Prometheus gathering, we'll use the [prometheus-net](https://github.com/prometheus-net/prometheus-net) project.

We have examples for the currently available metrics:

* Counter
* Gauge
* Summary 
* Histogram

Docs about [metric types](https://prometheus.io/docs/concepts/metric_types/) and [instrumentation best practices](https://prometheus.io/docs/practices/instrumentation/#counter-vs.-gauge-vs.-summary).

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

# restore paket tools
$ dotnet paket restore

# make sure you can build the project
$ dotnet build

# start the server
$ dotnet run
```

## How to test it locally

```bash
# you need to send this header
curl -X GET \
    -H "Accept: application/json" \
    localhost:8085/api/test

# this would be the real application endpoint
curl -X GET \
    -H "Accept: application/json" \
    localhost:8085/api

# trying the POST endpoint
curl -X POST \
    -H "Accept: application/json" \
    -d '{"id":"1", "message":"F#, Prometheus and Grafana"}' \
    localhost:8085/api
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