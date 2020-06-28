﻿namespace ForexMiner.Heimdallr.UserManager.Utilities
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Utilities.Data.User;
    using ForexMiner.Heimdallr.UserManager.Database;
    using System.Collections.Generic;

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
