using HelloOperator.Model.CustomResources;
using k8s;

namespace HelloOperator.Controller.Handlers;

public class AppDeletedHandler(Kubernetes kubernetes)
{
    private string _namespace = "default";

    public async Task<(bool IsDeploymentDeleted, bool IsConfigMapDeleted)> HandleAsync(HelloApp app, string @namespace)
    {
        _namespace = @namespace;
        return (IsDeploymentDeleted: await DeleteDeploymentAsync(app),
            IsConfigMapDeleted: await DeleteConfigMapAsync(app));
    }

    private async Task<bool> DeleteDeploymentAsync(HelloApp app)
    {
        var name = $"ha-{app.Metadata.Name}";
        var deployments = await kubernetes.ListNamespacedDeploymentAsync(
            _namespace, fieldSelector: $"metadata.name={name}");

        if (deployments.Items.Any())
        {
            await kubernetes.DeleteNamespacedDeploymentAsync(name, _namespace);
            Console.WriteLine($"    Deleted Deployment: {name}, in namespace: {_namespace}");
            return true;
        }

        Console.WriteLine($"    No Deployment to delete: {name}, in namespace: {_namespace}");
        return false;
    }

    private async Task<bool> DeleteConfigMapAsync(HelloApp pinger)
    {
        var name = $"ha-{pinger.Metadata.Name}-config";
        var configMaps = await kubernetes.ListNamespacedConfigMapAsync(
            _namespace, fieldSelector: $"metadata.name={name}");

        if (configMaps.Items.Any())
        {
            await kubernetes.DeleteNamespacedConfigMapAsync(name, _namespace);
            Console.WriteLine($"    Deleted ConfigMap: {name}, in namespace: {_namespace}");
            return true;
        }

        Console.WriteLine($"    No ConfigMap to delete: {name}, in namespace: {_namespace}");
        return false;
    }
}