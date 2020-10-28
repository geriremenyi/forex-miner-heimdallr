namespace ForexMiner.Heimdallr.Users.Api.Common
{
    using AutoMapper;
    using Contracts = Heimdallr.Common.Data.Contracts.User;
    using Database = Heimdallr.Common.Data.Database.Models.User;

    public class AutoMapping : Profile
    {

        public AutoMapping()
        {
            CreateMap<Database.User, Contracts.User>();
            CreateMap<Contracts.Registration, Database.User>();
            CreateMap<Database.User, Contracts.LoggedInUser>();
        }

    }
}
