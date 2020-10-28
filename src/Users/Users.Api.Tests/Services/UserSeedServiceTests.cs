namespace Users.Api.Tests.Services
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.User;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using Role = ForexMiner.Heimdallr.Common.Data.Database.Models.User.Role;
    using ForexMiner.Heimdallr.Users.Api.Services;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using System;
    using Xunit;

    public class UserSeedServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly IEntitySeedService _userSeedService;

        public UserSeedServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _userServiceMock = new Mock<IUserService>();
            _userSeedService = new UserSeedService(_configurationMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public void Seed_Exists()
        {
            // Arrange
            var adminUserMock = new User()
            {
                Id = Guid.NewGuid(),
                Email = "admin@unit-test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = Role.Admin,
                HasConnections = false
            };
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "SuperAdmin-Email")]).Returns(adminUserMock.Email);
            _userServiceMock.Setup(us => us.GetUserByEmail(adminUserMock.Email)).Returns(adminUserMock);

            // Act
            _userSeedService.Seed();

            // Assert
            _configurationMock.VerifyGet(c => c["SuperAdmin-Email"], Times.Once());
            _configurationMock.VerifyGet(c => c["SuperAdmin-Password"], Times.Never());
            _userServiceMock.Verify(us => us.Register(It.IsAny<Registration>(), It.IsAny<Role>()), Times.Never());
        }

        [Fact]
        public void Seed_DoesntExist()
        {
            // Arrange
            var adminUserMock = new User()
            {
                Id = Guid.NewGuid(),
                Email = "admin@unit-test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = Role.Admin,
                HasConnections = false
            };
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "SuperAdmin-Email")]).Returns(adminUserMock.Email);
            _configurationMock.SetupGet(c => c[It.Is<string>(cv => cv == "SuperAdmin-Password")]).Returns("OneShouldSimplyNotExposeAdminPasswordsInUnitTests");
            _userServiceMock.Setup(us => us.GetUserByEmail(adminUserMock.Email)).Returns(null as User);
            _userServiceMock.Setup(us => us.Register(It.IsAny<Registration>(), Role.Admin)).Returns(adminUserMock);

            // Act
            _userSeedService.Seed();

            // Assert
            _configurationMock.VerifyGet(c => c["SuperAdmin-Email"], Times.Once());
            _configurationMock.VerifyGet(c => c["SuperAdmin-Password"], Times.Once());
            _userServiceMock.Verify(us => us.Register(It.IsAny<Registration>(), Role.Admin), Times.Once());
        }
    }
}
