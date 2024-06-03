using HelloOperator.Model.CustomResources;
using k8s;
using k8s.Models;

namespace HelloOperator.Controller.Handlers;

public class AppAddedHandler(Kubernetes kubernetes)
{
    private string _namespace = "default";

    public async Task<(bool IsConfigMapCreated, bool IsDeploymentCreated)> HandleAsync(HelloApp app, string @namespace)
    {
        _namespace = @namespace;
        return (IsConfigMapCreated: await EnsureConfigMapAsync(app),
            IsDeploymentCreated: await EnsureDeploymentAsync(app));
    }

    private async Task<bool> EnsureConfigMapAsync(HelloApp app)
    {
        var name = $"ha-{app.Metadata.Name}-config";
        var configMaps = await kubernetes.ListNamespacedConfigMapAsync(
            _namespace, fieldSelector: $"metadata.name={name}");

        if (!configMaps.Items.Any())
        {
            var configMap = new V1ConfigMap
            {
                Metadata = new V1ObjectMeta
                {
                    Name = name,
                    Labels = new Dictionary<string, string>
                    {
                        { "app", "hello-app" },
                        { "for", $"ha-{app.Metadata.Name}" },
                    },
                },
                Data = new Dictionary<string, string>
                {
                    { "MESSAGE", app.Spec.Message },
                },
            };

            await kubernetes.CreateNamespacedConfigMapAsync(configMap, _namespace);
            Console.WriteLine($"    Created ConfigMap: {name}, in namespace: {_namespace}");
            return true;
        }

        Console.WriteLine($"    ConfigMap exists: {name}, in namespace: {_namespace}");
        return false;
    }

    private async Task<bool> EnsureDeploymentAsync(HelloApp app)
    {
        var name = $"ha-{app.Metadata.Name}";
        var deployments = await kubernetes.ListNamespacedDeploymentAsync(
            _namespace, fieldSelector: $"metadata.name={name}");

        if (!deployments.Items.Any())
        {
            var deployment = new V1Deployment
            {
                Metadata = new V1ObjectMeta
                {
                    Name = name,
                    Labels = new Dictionary<string, string>
                    {
                        { "app", "hello-app" },
                    },
                },
                Spec = new V1DeploymentSpec()
                {
                    Selector = new V1LabelSelector
                    {
                        MatchLabels = new Dictionary<string, string>
                        {
                            { "app", "hello-app" },
                            { "instance", name },
                        },
                    },
                    Template = new V1PodTemplateSpec()
                    {
                        Metadata = new V1ObjectMeta
                        {
                            Labels = new Dictionary<string, string>
                            {
                                { "app", "hello-app" },
                                { "instance", name },
                                { "name", app.Metadata.Name },
                            },
                        },
                        Spec = new V1PodSpec
                        {
                            AutomountServiceAccountToken = false,
                            Containers = new List<V1Container>
                            {
                                new V1Container
                                {
                                    Name = "app",
                                    Image = "hello-operator.app:v1.0",
                                    EnvFrom = new List<V1EnvFromSource>
                                    {
                                        new V1EnvFromSource
                                        {
                                            ConfigMapRef = new V1ConfigMapEnvSource
                                            {
                                                Name = $"ha-{app.Metadata.Name}-config",
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            };

            await kubernetes.CreateNamespacedDeploymentAsync(deployment, _namespace);
            Console.WriteLine($"    Created Deployment: {name}, in namespace: {_namespace}");
            return true;
        }

        Console.WriteLine($"    Deployment exists: {name}, in namespace: {_namespace}");
        return false;
    }
}