namespace ForexMiner.Heimdallr.ConfigurationManager.Services.Secret.Providers
{
    public interface ISecretProvider
    {
        public Types.Secret GetSecret(string key);
    }
}
