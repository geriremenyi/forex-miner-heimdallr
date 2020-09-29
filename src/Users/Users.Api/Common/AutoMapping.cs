namespace ForexMiner.Heimdallr.Users.Api.Common
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.User;
    using ForexMiner.Heimdallr.Users.Api.Database;

    public class AutoMapping : Profile
    {

        public AutoMapping()
        {
            CreateMap<User, UserDTO>();
            CreateMap<RegistrationDTO, User>();
            CreateMap<User, AuthenticationResponseDTO>();
        }

    }
}
