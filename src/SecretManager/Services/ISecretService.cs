namespace ForexMiner.Heimdallr.SecretManager.Services
{
    using ForexMiner.Heimdallr.Data.Secret;

    public interface ISecretService
    {
        public SecretDTO GetSecret(string secretNamespace, string secretName);
    }
}
