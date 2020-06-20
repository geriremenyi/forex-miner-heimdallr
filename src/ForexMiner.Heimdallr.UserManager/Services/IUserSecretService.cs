namespace ForexMiner.Heimdallr.UserManager.Services
{
    public interface IUserSecretService
    {
        public string GetJwtEncryptionSecret();
    }
}
