namespace ForexMiner.Heimdallr.ConfigurationManager.Services.Secret
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Cache.Services;
    using ForexMiner.Heimdallr.ConfigurationManager.Services.Secret.Providers;
    using ForexMiner.Heimdallr.Data.Secret;
    using System;
    using System.Threading.Tasks;

    public class SecretService : ISecretService
    {
        private readonly ISecretProvider _secretProvider;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public SecretService(ISecretProvider secretProvider, ICacheService cacheService, IMapper mapper)
        {
            _secretProvider = secretProvider;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<SecretDTO> GetSecret(string secretNamespace, string secretName)
        {
            var secret = await _cacheService.GetOrCreateCacheValue(Cache.Utilities.CacheType.Secret, secretNamespace, secretName, () =>
            {
                return _secretProvider.GetSecret(GetSecretKey(secretNamespace, secretName));
            }, Cache.Utilities.CacheCreateTarget.Both);

            return _mapper.Map<Data.Secret, SecretDTO>(secret);
        }

        private string GetSecretKey(string secretNamespace, string secretName, Guid? ownerId = null) => ownerId == null ? $"{secretNamespace}:{secretName}" : $"{secretNamespace}:{secretName}:{ownerId:N}";
    }
}
