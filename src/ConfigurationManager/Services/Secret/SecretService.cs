namespace ForexMiner.Heimdallr.ConfigurationManager.Services.Secret
{
    using ForexMiner.Heimdallr.ConfigurationManager.Services.Secret.Providers;
    using ForexMiner.Heimdallr.Data.Secret;

    public class SecretService : ISecretService
    {
        private readonly ISecretProvider keyVaultSecretProvider;

        public SecretService(ISecretProvider keyVaultSecretProvider)
        {
            
        }

        public SecretDTO GetSecret(string secretNamespace, string secretName)
        {
            throw new System.NotImplementedException();
        }
    }
}
