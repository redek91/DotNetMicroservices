# This line should not be in a prod deployment for obvious security reasons
kubectl create namespace dotnetmicroservice
kubectl --namespace dotnetmicroservice create secret generic mssql --from-literal=SA_PASSWORD="meisterKey1$"

kubectl apply -f "K8S/nginx-depl.yaml"
kubectl --namespace dotnetmicroservice apply -f "K8S/local-pvc.yaml"
kubectl --namespace dotnetmicroservice apply -f "K8S/mssql-plat-depl.yaml"
kubectl --namespace dotnetmicroservice apply -f "K8S/rabbitmq-depl.yaml"
kubectl --namespace dotnetmicroservice apply -f "K8S/commands-depl.yaml"
kubectl --namespace dotnetmicroservice apply -f "K8S/platforms-depl.yaml"
kubectl --namespace dotnetmicroservice apply -f "K8S/ingress-srv.yaml"