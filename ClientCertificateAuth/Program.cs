using ClientCertificateAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureServices(s =>
    {
        s.AddSingleton<CertificateValidator>();
    })
    .ConfigureFunctionsWorkerDefaults(workerApplication =>
    {
        workerApplication.UseMiddleware<ClientCertificateMiddleware>();
    })
    .Build();

host.Run();