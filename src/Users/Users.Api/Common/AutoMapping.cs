namespace ForexMiner.Heimdallr.Users.Api.Common
{
    using AutoMapper;
    using Contracs = ForexMiner.Heimdallr.Common.Data.Contracts.User;
    using Database = ForexMiner.Heimdallr.Common.Data.Database.Models;

    public class AutoMapping : Profile
    {

        public AutoMapping()
        {
            CreateMap<Database.User, Contracs.User>();
            CreateMap<Contracs.Registration, Database.User>();
            CreateMap<Database.User, Contracs.LoggedInUser>();
        }

    }
}
