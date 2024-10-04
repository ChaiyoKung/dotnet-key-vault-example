using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
  .AddUserSecrets<Program>()
  .Build();

SecretClientOptions options = new SecretClientOptions()
{
  Retry =
  {
    Delay = TimeSpan.FromSeconds(2),
    MaxDelay = TimeSpan.FromSeconds(16),
    MaxRetries = 5,
    Mode = RetryMode.Exponential
  }
};
var keyVaultUri = config["KeyVault:Uri"] ?? throw new ArgumentNullException("KeyVault:Uri configuration is missing");
var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential(), options);

try
{
  KeyVaultSecret secret = client.GetSecret("my-secret");
  Console.WriteLine(secret.Value);
}
catch (Exception ex)
{
  Console.WriteLine(ex.Message);
}
