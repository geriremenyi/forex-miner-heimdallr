using ForexMiner.Heimdallr.Data.Secret;

namespace ForexMiner.Heimdallr.SecretManager.Services
{
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
