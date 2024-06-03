To deploy the application, launch Docker Engine and execute the following commands in sequence:

1. Build container images for the operator and the sample application by running the following command from the solution directory:

```bash
dotnet publish /t:PublishContainer
```

2. Change terminal directory to the `spec` and deploy the RBAC policy and the operator by running the following command:

```bash
kubectl apply -f rbac.yaml
kubectl apply -f operator.yaml
```

3. Deploy the sample application by running the following command:

```bash
kubectl apply -f hello-app.yaml
```

4. The previous command will deploy 2 pods with configmaps containing the message that the application prints to its logs every 30 seconds. To delete the application, run the following command:

```bash
kubectl delete -f hello-app.yaml
```

