namespace Users.Api.Tests.Services
{
    using AutoMapper;
    using Contracts = ForexMiner.Heimdallr.Common.Data.Contracts.User;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using Database = ForexMiner.Heimdallr.Common.Data.Database.Models.User;
    using ForexMiner.Heimdallr.Users.Api.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using ForexMiner.Heimdallr.Common.Data.Mapping;
    using Microsoft.AspNetCore.Mvc;
    using ForexMiner.Heimdallr.Common.Data.Exceptions;
    using System.Net;

    public class UserServiceTests: IDisposable
    {
        private readonly ForexMinerHeimdallrDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            // Setup an actual in-memory Sqlite for db mocking
            var optionsBuilder = new DbContextOptionsBuilder<ForexMinerHeimdallrDbContext>();
            optionsBuilder.UseSqlite("Filename=:memory:");
            _dbContext = new ForexMinerHeimdallrDbContext(optionsBuilder.Options);
            _dbContext.Database.OpenConnection();
            _dbContext.Database.Migrate();

            // Auto mapper
            var contractContract = new ContractContractMappings();
            var databaseContract = new DatabaseContractMappings();
            var oandaContract = new OandaContractMappings();
            var configuration = new MapperConfiguration(cfg => {
                cfg.AddProfile(contractContract);
                cfg.AddProfile(databaseContract);
                cfg.AddProfile(oandaContract);
            });
            _mapper = new Mapper(configuration);

            // Mocks
            _configurationMock = new Mock<IConfiguration>();

            // Class under test
            _userService = new UserService(_dbContext, _mapper, _configurationMock.Object);
        }

        [Fact]
        public void Login()
        {
            // Arrange
            var userLogin = new Contracts.UserLogin()
            {
                Email = "unit@test.ai",
                Password = "ThisIsAPassword"
            };
            var user = new Database.User()
            {
                Id = Guid.NewGuid(),
                Email = userLogin.Email,
                FirstName = "Unit",
                LastName = "Test",
                Role = Database.Role.Trader
            };
            user.UpdatePassword(userLogin.Password);
            _dbContext.Add(user);
            _dbContext.SaveChanges();
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "JWT-IssuerSigningKey")]).Returns("aYPg2QjKQBY4Uqx8");

            // Act
            var loggedInUser = _userService.Login(userLogin);

            // Assert
            _configurationMock.VerifyGet(c => c["JWT-IssuerSigningKey"], Times.Once());
            Assert.Equal(loggedInUser.Email, userLogin.Email);
        }

        [Fact]
        public void Login_NotFound()
        {
            // Arrange
            var userLogin = new Contracts.UserLogin()
            {
                Email = "unit@test.ai",
                Password = "ThisIsAPassword"
            };

            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _userService.Login(userLogin));
            Assert.Equal(HttpStatusCode.NotFound, exception.Status);
            _configurationMock.VerifyGet(c => c["JWT-IssuerSigningKey"], Times.Never());
        }

        [Fact]
        public void Login_InvalidPassword()
        {
            // Arrange
            var userLogin = new Contracts.UserLogin()
            {
                Email = "unit@test.ai",
                Password = "ThisIsAPassword"
            };
            var user = new Database.User()
            {
                Id = Guid.NewGuid(),
                Email = userLogin.Email,
                FirstName = "Unit",
                LastName = "Test",
                Role = Database.Role.Trader
            };
            user.UpdatePassword("AndThisIsAsWellButDifferent");
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _userService.Login(userLogin));
            Assert.Equal(HttpStatusCode.NotFound, exception.Status);
            _configurationMock.VerifyGet(c => c["JWT-IssuerSigningKey"], Times.Never());
        }

        [Fact]
        public void Register_Wo_Role()
        {
            // Arrange
            var registrationRequest = new Contracts.Registration()
            {
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Password = "ThisIsAPassword"
            };

            // Act
            var registration = _userService.Register(registrationRequest);

            // Assert
            Assert.Equal(registrationRequest.Email, registration.Email);
            Assert.Equal(registrationRequest.FirstName, registration.FirstName);
            Assert.Equal(registrationRequest.LastName, registration.LastName);
            Assert.Equal(Database.Role.Trader, registration.Role);
        }

        [Fact]
        public void Register_With_Role()
        {
            // Arrange
            var registrationRequest = new Contracts.Registration()
            {
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Password = "ThisIsAPassword"
            };

            // Act
            var registration = _userService.Register(registrationRequest, Database.Role.Admin);

            // Assert
            Assert.Equal(registrationRequest.Email, registration.Email);
            Assert.Equal(registrationRequest.FirstName, registration.FirstName);
            Assert.Equal(registrationRequest.LastName, registration.LastName);
            Assert.Equal(Database.Role.Admin, registration.Role);
        }

        [Fact]
        public void Register_EmailAlradyExists()
        {
            // Arrange
            var registration = new Contracts.Registration()
            {
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Password = "ThisIsAPassword"
            };
            var user = new Database.User()
            {
                Id = Guid.NewGuid(),
                Email = registration.Email,
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                Role = Database.Role.Trader
            };
            user.UpdatePassword("PasswordBecauseItIsRequired");
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _userService.Register(registration));
            Assert.Equal(HttpStatusCode.BadRequest, exception.Status);
        }

        [Fact]
        public void GetUserById()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = new Database.User()
            {
                Id = id,
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = Database.Role.Trader
            };
            user.UpdatePassword("Whatever");
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            var userById = _userService.GetUserById(id);

            // Assert
            Assert.Equal(user.Id, userById.Id);
            Assert.Equal(user.Email, userById.Email);
            Assert.Equal(user.FirstName, userById.FirstName);
            Assert.Equal(user.LastName, userById.LastName);
            Assert.Equal(user.Role, userById.Role);
        }

        [Fact]
        public void GetUserById_NotFound()
        {
            // Arrange
            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _userService.GetUserById(Guid.NewGuid()));
            Assert.Equal(HttpStatusCode.NotFound, exception.Status);
        }

        [Fact]
        public void GetUserByEmail()
        {
            // Arrange
            var email = "unit@test.ai";
            var user = new Database.User()
            {
                Id = Guid.NewGuid(),
                Email = email,
                FirstName = "Unit",
                LastName = "Test",
                Role = Database.Role.Trader
            };
            user.UpdatePassword("Whatever");
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            var userById = _userService.GetUserByEmail(email);

            // Assert
            Assert.Equal(user.Id, userById.Id);
            Assert.Equal(user.Email, userById.Email);
            Assert.Equal(user.FirstName, userById.FirstName);
            Assert.Equal(user.LastName, userById.LastName);
            Assert.Equal(user.Role, userById.Role);
        }

        [Fact]
        public void UpdateUserById()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateUser = new Contracts.UserUpdate()
            {
                Email = "update@this.ai",
                FirstName = "Iam",
                LastName = "Different",
                Password = "EvenThePasswordIs"
            };
            var user = new Database.User()
            {
                Id = id,
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = Database.Role.Trader
            };
            user.UpdatePassword("Whatever");
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            var updatedUser = _userService.UpdateUserById(id, updateUser);

            // Assert
            Assert.Equal(updateUser.Email, updatedUser.Email);
            Assert.Equal(updateUser.FirstName, updatedUser.FirstName);
            Assert.Equal(updateUser.LastName, updatedUser.LastName);
        }

        [Fact]
        public void UpdateUserById_NotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateUser = new Contracts.UserUpdate()
            {
                Email = "update@this.ai",
                FirstName = "Iam",
                LastName = "Different",
                Password = "EvenThePasswordIs"
            };

            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _userService.UpdateUserById(id, updateUser));
            Assert.Equal(HttpStatusCode.NotFound, exception.Status);
        }

        [Fact]
        public void DeleteUserById()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = new Database.User()
            {
                Id = id,
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = Database.Role.Trader
            };
            user.UpdatePassword("Whatever");
            _dbContext.Add(user);
            _dbContext.SaveChanges();

            // Act
            _userService.DeleteUserById(id);

            // Assert
            var userWhichWasInDb = _dbContext.Users.Where(user => user.Id == id).SingleOrDefault();
            Assert.Null(userWhichWasInDb);
        }

        [Fact]
        public void DeleteUserById_NotFound()
        {
            // Arrange
            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _userService.DeleteUserById(Guid.NewGuid()));
            Assert.Equal(HttpStatusCode.NotFound, exception.Status);
        }

        [Fact]
        public void GetAllUsers()
        {
            // Arrange
            var user1 = new Database.User()
            {
                Id = Guid.NewGuid(),
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = Database.Role.Trader
            };
            user1.UpdatePassword("Whatever");
            _dbContext.Add(user1);

            var user2 = new Database.User()
            {
                Id = Guid.NewGuid(),
                Email = "unit2@test.ai",
                FirstName = "Unit2",
                LastName = "Test2",
                Role = Database.Role.Admin
            };
            user2.UpdatePassword("Different");
            _dbContext.Add(user2);

            _dbContext.SaveChanges();

            // Act
            var users = _userService.GetAllUsers();

            // Assert
            users = users.OrderBy(user => user.Email);
            Assert.Equal(2, users.Count());
            Assert.Equal(user1.Email, users.ElementAt(0).Email);
            Assert.Equal(user2.Email, users.ElementAt(1).Email);
        }

        public void Dispose()
        {
            _dbContext.Database.CloseConnection();
        }
    }
}
