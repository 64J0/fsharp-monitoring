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