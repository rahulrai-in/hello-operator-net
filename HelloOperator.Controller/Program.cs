using HelloOperator.Controller.Handlers;
using HelloOperator.Model.CustomResources;
using k8s;

var kubeConfig = KubernetesClientConfiguration.BuildDefaultConfig();
var kubernetes = new Kubernetes(kubeConfig);

Console.WriteLine("Watching for HelloApp events");

GenericClient gc = new(kubernetes, HelloApp.Definition.Group, HelloApp.Definition.Version, HelloApp.Definition.Plural);
await foreach (var item in gc.WatchNamespacedAsync<HelloApp>("default"))
{
    switch (item.Item1)
    {
        case WatchEventType.Added:
            Console.WriteLine($"* Handling creation of {item.Item2.Metadata.Name}");
            var (isConfigMapCreated, isDeploymentCreated) =
                await new AppAddedHandler(kubernetes).HandleAsync(item.Item2, "default");
            Console.WriteLine(
                $"    * ConfigMap created: {isConfigMapCreated}, Deployment created: {isDeploymentCreated}");
            break;
        case WatchEventType.Deleted:
            Console.WriteLine($"* Handling deletion of {item.Item2.Metadata.Name}");
            var (isDeploymentDeleted, isConfigMapDeleted) =
                await new AppDeletedHandler(kubernetes).HandleAsync(item.Item2, "default");
            Console.WriteLine(
                $"    * ConfigMap deleted: {isConfigMapDeleted}, Deployment deleted: {isDeploymentDeleted}");
            break;
        default:
            Console.WriteLine($"* Event not handled: {item.Item1}");
            break;
    }
}