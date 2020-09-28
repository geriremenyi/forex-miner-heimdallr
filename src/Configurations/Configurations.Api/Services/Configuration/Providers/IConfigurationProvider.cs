namespace ForexMiner.Heimdallr.Configurations.Api.Services.Configuration.Providers
{
    public interface IConfigurationProvider
    {
        public Types.Configuration GetConfiguration(string key);
    }
}
