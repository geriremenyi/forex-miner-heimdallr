namespace ForexMiner.Heimdallr.ConfigurationManager.Services.Secret
{
    using ForexMiner.Heimdallr.Utilities.Data.Configuration.Secret;
    using System.Threading.Tasks;

    public class SecretService : ISecretService
    {
        public Task<SecretDTO> GetSecret(string secretNamespace, string secretName)
        {
            throw new System.NotImplementedException();
        }
    }
}
