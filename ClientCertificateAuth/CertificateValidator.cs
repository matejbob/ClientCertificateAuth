using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace ClientCertificateAuth
{
    internal class CertificateValidator
    {
        public CertificateValidator(ILogger<CertificateValidator> logger)
        {
            _logger = logger;
        }

        public async Task<bool> IsAnyCertificateValid(IEnumerable<string>? certificates)
        {
            try
            {
                if (certificates == null || certificates.Count() == 0)
                {
                    return false;
                }

                await Task.Delay(0); //simulating call to an external service to obain list of allowed certificate thumbprints

                foreach (var cert in certificates)
                {
                    try
                    {
                        var certBytes = Convert.FromBase64String(cert);
                        var certificate = new X509Certificate2(certBytes);

                        if (validCertificates.Contains(certificate.Thumbprint))
                        {
                            return true;
                        }
                    }
                    catch (Exception exc)
                    {
                        _logger.LogDebug($"Failed to validate certificate ${cert}. Exception: ${exc}");
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.LogDebug($"Error during client certificate validation. Exception: ${exc}");
            }

            return false;
        }

        private readonly HashSet<string> validCertificates = new()
        {
            "EF7D48E4A3B44FBCFE13881EC8504E13B51D2EF2"
        };

        private readonly ILogger<CertificateValidator> _logger;
    }
}
