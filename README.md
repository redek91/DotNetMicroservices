# DotNetMicroservices

Simple Microservice Architecture

Following YT Course: [.NET Microservices – Full Course | Les Jackson](https://www.youtube.com/watch?v=DgVjEo3OGBI&t=3874s&ab_channel=LesJackson)

## Project Summary

This solution has 2 Microservices (Platforms, Commands) and is used as a proof of concept to:

- Explore Kubernetes
- GRPC

A Platform is a tecnology entity (like docker or Nutanix) and a Command is a Command that can be executed on a Platform. It is possible to create a Command using the CommandsAPI(ms) that has entries that are passed by MessageBus communication or GRPC. A new Platform can be created using the PlatformsAPI(ms), that is used as a inventory for all platforms. The CommandsAPI is used as a command line inventory.

In the solution there are examples for 3 data exchange types:

- Synchronous exchange: the PlatformsAPI posts new Platforms to the CommandsAPI per POST/PUT request
- Asynchronous exchange: the PlatformsAPI publishes an Event to a MessageBus(RabbitMQ) and the CommandsAPI consumes the Event to create a new Platform
- GRPC: Synchronous exchange used to retrieve Platforms from the PlatformsApi to seed data in the CommandsAPI

Note: the containers are communicating using the DNS names specified in the ClusterIp services ex: "platforms-clusterip-srv"

## Kubernetes

- Pod: A pod is a placeholder for a container instance and it ensures that the instance is running (retry)
- Service ClusterIp: is used to communicate between container instances
- PVC (Persistent Volume Claim): is used to store persistent data
- Deployment: a deployment is a manifest that specifies a container/containers instance
- Service - NodePort: a direct pipeline to communicate to a container instance (for development)
- Service - LoadBalancer: maps to a container instance (communication) and uses round robin if a deployment has more than 1 replica
- Ingress: is used to map APIs to external IPs (used to create an API gateway) - works with ingress-nginx
- ingress-nginx: Is used to create an API Gateway + Loadbalancer
- Secret: used to store sensible data to avoid password/credentials data leaks
- kubectl: command to manage Kubernetes

## Usefull commands

- `dotnet add {projectFolder} package {packagename}`: add package from solution folder
- `dotnet dev-certs https --trust`: add development certificates to keystore (to trust local https)
- `kubectl [--namespace {namespace}] get deployments`: lists all deployments and status
- `kubectl [--namespace {namespace}] get services`: lists all services
- `kubectl [--namespace {namespace}] get pods`: lists all pods and status
- `kubectl [--namespace {namespace}] get pvc`: lists all persistent storage claims and status
- `kubectl [--namespace {namespace}] delete {type, ex:service} {name}`: deletes objects
- `kubectl [--namespace {namespace}] rollout restart deployment {name}`: restarts a deployment (used to download docker images again -> upgrades), used if yaml file has not changed, but the docker image has

!!Check PDF in docs for usefull K8S & Docker commands

- ### Apply SQL Server Migrations

```
dotnet ef migrations add InitialMigration -p .\PlatformsService\ -s .\PlatformsService\ -o Data/Migrations -- --environment Production
```

## Config

- Sql Server pass: meisterKey1$

To create K8S secret:

```
kubectl create secret generic mssql --from-literal=SA_PASSWORD="meisterKey1$"
```

## External Services

- ### ingress-nginx (API-Gateway)

```
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.2.0/deploy/static/provider/cloud/deploy.yaml
```

## Todo

- [ ] Persist Rabbit MQ
- [ ] Use MassTransit
- [ ] Clean Architecture
- [ ] Autoscaler in K8S
- [ ] Try a multi-node K8S infrastructure
- [ ] Deployment priority (Deploy CommandsApi only if PlatformsApi already runs, possible???)
