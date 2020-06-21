namespace ForexMiner.Heimdallr.UserManager.Services
{
    using System.Threading.Tasks;

    public interface IUserSecretService
    {
        public Task<string> GetJwtEncryptionSecret();
    }
}
