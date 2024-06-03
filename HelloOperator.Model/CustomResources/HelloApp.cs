using k8s;
using k8s.Models;

namespace HelloOperator.Model.CustomResources;

public class HelloApp : KubernetesObject
{
    public V1ObjectMeta Metadata { get; set; }

    public AppSpec Spec { get; set; }

    public class AppSpec
    {
        public string Message { get; set; }
    }

    public struct Definition
    {
        public const string Group = "operators.rahulrai.net";
        public const string Version = "v1";
        public const string Plural = "helloapp";
        public const string Singular = "helloapps";
        public const string Kind = nameof(HelloApp);
        public const string ShortName = "ha";
    }
}