namespace ForexMiner.Heimdallr.Configurations.Api.Services.Secret
{
    using ForexMiner.Heimdallr.Common.Data.Configuration.Secret;
    using System.Threading.Tasks;

    public class SecretService : ISecretService
    {
        public Task<SecretDTO> GetSecret(string secretNamespace, string secretName)
        {
            throw new System.NotImplementedException();
        }
    }
}
