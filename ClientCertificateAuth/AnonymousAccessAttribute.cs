namespace ClientCertificateAuth
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]

    public class AnonymousAccessAttribute : Attribute
    {
    }
}