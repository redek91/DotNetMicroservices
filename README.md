# DotNetMicroservices

Simple Microservice Architecture

Following YT Course: [.NET Microservices – Full Course | Les Jackson](https://www.youtube.com/watch?v=DgVjEo3OGBI&t=3874s&ab_channel=LesJackson)

## Usefull commands

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
kubectl apply -f kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.2.0/deploy/static/provider/cloud/deploy.yaml
```

## Todo

- [ ] Persist Rabbit MQ
- [ ] Use MassTransit
- [ ] Clean Architecture
- [ ] Autoscaler in K8S
