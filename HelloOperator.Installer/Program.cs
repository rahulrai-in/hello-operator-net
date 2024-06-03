using HelloOperator.Installer;
using k8s;

var kubeConfig = KubernetesClientConfiguration.BuildDefaultConfig();

var kubernetes = new Kubernetes(kubeConfig);

var installer = new HelloAppInstaller(kubernetes);
await installer.InstallAsync();

Console.WriteLine("Done.");