using HelloOperator.Model.CustomResources;
using k8s;
using k8s.Models;

namespace HelloOperator.Installer;

public class HelloAppInstaller(Kubernetes kubernetes)
{
    public async Task InstallAsync()
    {
        await EnsureHelloAppCrdAsync();
    }

    private async Task EnsureHelloAppCrdAsync()
    {
        var crds = await kubernetes.ListCustomResourceDefinitionAsync(
            fieldSelector: $"metadata.name={HelloApp.Definition.Plural}.{HelloApp.Definition.Group}");

        if (!crds.Items.Any())
        {
            var crd = new V1CustomResourceDefinition
            {
                Metadata = new V1ObjectMeta ()
                {
                    Name = $"{HelloApp.Definition.Plural}.{HelloApp.Definition.Group}",
                    Labels = new Dictionary<string, string>
                    {
                        { "app", "hello-operator" },
                        { "author", "rahulrai" },
                    },
                },
                Spec = new V1CustomResourceDefinitionSpec ()
                {
                    Group = HelloApp.Definition.Group,
                    Scope = "Namespaced",
                    Names = new V1CustomResourceDefinitionNames ()
                    {
                        Plural = HelloApp.Definition.Plural,
                        Singular = HelloApp.Definition.Singular,
                        Kind = HelloApp.Definition.Kind,
                        ShortNames = new List<string>
                        {
                            HelloApp.Definition.ShortName,
                        },
                    },
                    Versions = new List<V1CustomResourceDefinitionVersion>
                    {
                        new V1CustomResourceDefinitionVersion ()
                        {
                            Name = "v1",
                            Served = true,
                            Storage = true,
                            Schema = new V1CustomResourceValidation ()
                            {
                                OpenAPIV3Schema = new V1JSONSchemaProps ()
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, V1JSONSchemaProps>
                                    {
                                        {
                                            "spec", new V1JSONSchemaProps
                                            {
                                                Type = "object",
                                                Properties = new Dictionary<string, V1JSONSchemaProps>
                                                {
                                                    {
                                                        "message", new V1JSONSchemaProps
                                                        {
                                                            Type = "string",
                                                        }
                                                    },
                                                },
                                            }
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            };

            await kubernetes.CreateCustomResourceDefinitionAsync(crd);
            Console.WriteLine(
                $"** Created CRD for Kind: {HelloApp.Definition.Kind}; ApiVersion: {HelloApp.Definition.Group}/{HelloApp.Definition.Version}");
        }
        else
        {
            Console.WriteLine(
                $"** CRD already exists for Kind: {HelloApp.Definition.Kind}; ApiVersion: {HelloApp.Definition.Group}/{HelloApp.Definition.Version}");
        }
    }
}