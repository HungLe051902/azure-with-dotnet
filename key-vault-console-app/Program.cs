using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;

namespace key_vault_console_app
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.Delay(1000);
            Console.WriteLine("Start.");
        }

        static async Task InteractWithAzureSecret(string keyVaultName = "vault-gmitvequycioz", string secretName = "SqlConnectionString")
        {
            var kvUri = $"https://{keyVaultName}.vault.azure.net";

            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            Console.Write("Input the value of your secret > ");
            var secretValue = Console.ReadLine();

            Console.Write($"Creating a secret in {keyVaultName} called '{secretName}' with the value '{secretValue}' ...");
            await client.SetSecretAsync(secretName, secretValue);
            Console.WriteLine(" done.");

            Console.WriteLine("Forgetting your secret.");
            secretValue = string.Empty;
            Console.WriteLine($"Your secret is '{secretValue}'.");

            Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
            var secret = await client.GetSecretAsync(secretName);
            Console.WriteLine($"Your secret is '{secret.Value.Value}'.");

            Console.Write($"Deleting your secret from {keyVaultName} ...");
            DeleteSecretOperation operation = await client.StartDeleteSecretAsync(secretName);
            // You only need to wait for completion if you want to purge or recover the secret.
            await operation.WaitForCompletionAsync();
            Console.WriteLine(" done.");

            Console.Write($"Purging your secret from {keyVaultName} ...");
            await client.PurgeDeletedSecretAsync(secretName);
            Console.WriteLine(" done.");
        }

        static async Task InteractWithAzureKey(string keyVaultName = "vault-gmitvequycioz", string keyName = "myKey")
        {
            var kvUri = $"https://{keyVaultName}.vault.azure.net";
            var client = new KeyClient(new Uri(kvUri), new DefaultAzureCredential());

            Console.Write($"Creating a key in {keyVaultName} called '{keyName}' ...");
            var createdKey = await client.CreateKeyAsync(keyName, KeyType.Rsa);
            Console.WriteLine("done.");

            Console.WriteLine($"Retrieving your key from {keyVaultName}.");
            var key = await client.GetKeyAsync(keyName);
            Console.WriteLine($"Your key version is '{key.Value.Properties.Version}'.");

            Console.Write($"Deleting your key from {keyVaultName} ...");
            var deleteOperation = await client.StartDeleteKeyAsync(keyName);
            // You only need to wait for completion if you want to purge or recover the key.
            await deleteOperation.WaitForCompletionAsync();
            Console.WriteLine("done.");

            Console.Write($"Purging your key from {keyVaultName} ...");
            await client.PurgeDeletedKeyAsync(keyName);
            Console.WriteLine(" done.");
        }
    }
}