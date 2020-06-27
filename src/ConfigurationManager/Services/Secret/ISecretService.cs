namespace ForexMiner.Heimdallr.ConfigurationManager.Services.Secret
{
    using ForexMiner.Heimdallr.Data.Secret;

    public interface ISecretService
    {
        public SecretDTO GetSecret(string secretNamespace, string secretName);
    }
}
