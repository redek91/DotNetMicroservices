# This line should not be in a prod deployment for obvious security reasons
kubectl create secret generic mssql --from-literal=SA_PASSWORD="meisterKey1$"

kubectl apply -f "K8S/nginx-depl.yaml"
kubectl apply -f "K8S/local-pvc.yaml"
kubectl apply -f "K8S/mssql-depl.yaml"
kubectl apply -f "K8S/rabbitmq-depl.yaml"
kubectl apply -f "K8S/commands-depl.yaml"
kubectl apply -f "K8S/platform-depl.yaml"
kubectl apply -f "K8S/ingress-srv.yaml"