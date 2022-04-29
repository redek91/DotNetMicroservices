
kubectl delete service commands-clusterip-srv
kubectl delete service mssql-clusterip-srv
kubectl delete service mssql-loadbalancer
kubectl delete service platforms-clusterip-srv
kubectl delete service rabbitmq-clusterip-srv
kubectl delete service rabbitmq-loadbalancer

kubectl delete deployment commands-depl
kubectl delete deployment platforms-depl
kubectl delete deployment rabbitmq-depl
kubectl delete deployment mssql-depl

kubectl delete ingress ingress-srv

kubectl delete namespace ingress-nginx

kubectl delete pvc mssql-claim

kubectl delete secret mssql