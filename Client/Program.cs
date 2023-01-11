using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {

            const string url = @"https://clientcertauthsample.azurewebsites.net/api/SecuredFunction";

            using (var handler = new HttpClientHandler())
            {
                var cert = new X509Certificate2("allowed.pfx", "Pa@@word123");
                handler.ClientCertificates.Add(cert);
                using (var httpClient = new HttpClient(handler))
                {
                    var response = await httpClient.GetAsync(url);
                    Console.WriteLine($"INFO: response = {response}");
                    Console.WriteLine($"INFO: response content = {await response.Content.ReadAsStringAsync()}");
                }
            }

            Console.WriteLine("INFO: finished");
        }
        catch (Exception exc)
        {
            Console.WriteLine($"ERROR: ${exc}");
        }
        finally
        {
            Console.ReadKey();
        }
    }
}