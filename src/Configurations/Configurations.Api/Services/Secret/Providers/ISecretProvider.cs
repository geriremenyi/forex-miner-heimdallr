namespace ForexMiner.Heimdallr.Configurations.Api.Services.Secret.Providers
{
    public interface ISecretProvider
    {
        public Types.Secret GetSecret(string key);
    }
}
