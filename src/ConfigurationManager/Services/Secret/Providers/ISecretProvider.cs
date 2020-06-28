namespace ForexMiner.Heimdallr.ConfigurationManager.Services.Secret.Providers
{
    using ForexMiner.Heimdallr.ConfigurationManager.Data;

    public interface ISecretProvider
    {
        public Secret GetSecret(string key);
        public Secret CreateSecret(string key, Secret value);
        public Secret UpdateSecret(string key, Secret value);
        public void DeleteSecret(string key);
    }
}
