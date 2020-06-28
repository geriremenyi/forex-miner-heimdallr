namespace ForexMiner.Heimdallr.ConfigurationManager.Services.Secret
{
    using ForexMiner.Heimdallr.Data.Secret;
    using System.Threading.Tasks;

    public interface ISecretService
    {
        public Task<SecretDTO> GetSecret(string secretNamespace, string secretName);
    }
}
