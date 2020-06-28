namespace ForexMiner.Heimdallr.ConfigurationManager.Services.Configuration.Providers
{
    public interface IConfigurationProvider
    {
        public Types.Configuration GetConfiguration(string key);
    }
}
