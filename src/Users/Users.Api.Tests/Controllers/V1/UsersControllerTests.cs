namespace Users.Api.Tests.Controllers.V1
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.User;
    using Role = ForexMiner.Heimdallr.Common.Data.Database.Models.User.Role;
    using ForexMiner.Heimdallr.Users.Api.Controllers.V1;
    using ForexMiner.Heimdallr.Users.Api.Services;
    using Moq;
    using System;
    using Xunit;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ForexMiner.Heimdallr.Common.Data.Exceptions;
    using System.Net;

    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _userController;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _userController = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public void Login()
        {
            // Arrange
            var userLogin = new UserLogin()
            {
                Email = "unit@test.ai",
                Password = "PssstItIsASecret!"
            };
            var mockLoggedInUser = new LoggedInUser()
            {
                Id = Guid.NewGuid(),
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = Role.Trader,
                HasConnections = false,
                Token = "this-is-a-jwt-token-believe-it-or-not"
            };
            _userServiceMock.Setup(us => us.Login(userLogin)).Returns(mockLoggedInUser);

            // Act
            var loggedInUser = _userController.Login(userLogin);

            // Assert
            _userServiceMock.Verify(us => us.Login(userLogin), Times.Once());
            Assert.Equal(mockLoggedInUser, loggedInUser);
        }

        [Fact]
        public void Register()
        {
            // Arrange
            var userRegistration = new Registration()
            {
                Email = "unit@test.ai",
                Password = "PssstItIsASecret!",
                FirstName = "Unit",
                LastName = "Test"
            };
            var mockRegisteredUser = new User()
            {
                Id = Guid.NewGuid(),
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = Role.Trader,
                HasConnections = false
            };
            _userServiceMock.Setup(us => us.Register(userRegistration, It.IsAny<Role>())).Returns(mockRegisteredUser);

            // Act
            var registeredUser = _userController.Register(userRegistration);

            // Assert
            _userServiceMock.Verify(us => us.Register(userRegistration, It.IsAny<Role>()), Times.Once());
            Assert.Equal(mockRegisteredUser, registeredUser);
        }

        [Fact]
        public void Me()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = Role.Trader;
            var mockMe = new User()
            {
                Id = userId,
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = role,
                HasConnections = false
            };
            MockUserContext(userId, role);
            _userServiceMock.Setup(us => us.GetUserById(userId)).Returns(mockMe);

            // Act
            var me = _userController.Me();

            // Assert
            _userServiceMock.Verify(us => us.GetUserById(userId), Times.Once());
            Assert.Equal(mockMe, me);
        }

        [Fact]
        public void GetUser_SameUserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = Role.Trader;
            var mockUser = new User()
            {
                Id = userId,
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = role,
                HasConnections = false
            };
            MockUserContext(userId, role);
            _userServiceMock.Setup(us => us.GetUserById(userId)).Returns(mockUser);

            // Act
            var user = _userController.GetUser(userId);

            // Assert
            _userServiceMock.Verify(us => us.GetUserById(userId), Times.Once());
            Assert.Equal(mockUser, user);
        }

        [Fact]
        public void GetUser_Admin()
        {
            // Arrange
            var requesterUserId = Guid.NewGuid();
            var requesterRole = Role.Admin;
            var userId = Guid.NewGuid();
            var role = Role.Trader;
            var mockUser = new User()
            {
                Id = userId,
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = role,
                HasConnections = false
            };
            MockUserContext(requesterUserId, requesterRole);
            _userServiceMock.Setup(us => us.GetUserById(userId)).Returns(mockUser);

            // Act
            var user = _userController.GetUser(userId);

            // Assert
            _userServiceMock.Verify(us => us.GetUserById(userId), Times.Once());
            Assert.Equal(mockUser, user);
        }

        [Fact]
        public void GetUser_DifferentUserId()
        {
            // Arrange
            var requesterUserId = Guid.NewGuid();
            var requesterRole = Role.Trader;
            var userId = Guid.NewGuid();
            var role = Role.Trader;
            var mockUser = new User()
            {
                Id = userId,
                Email = "unit@test.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = role,
                HasConnections = false
            };
            MockUserContext(requesterUserId, requesterRole);
            _userServiceMock.Setup(us => us.GetUserById(userId)).Returns(mockUser);

            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _userController.GetUser(userId));
            Assert.Equal(HttpStatusCode.Forbidden, exception.Status);
            _userServiceMock.Verify(us => us.GetUserById(It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public void Update_SameUserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = Role.Trader;
            var userUpdate = new UserUpdate()
            {
                Email = "unit@forex-miner.ai",
                FirstName = "Unit",
                LastName = "Test",
                Password = "IWantANewPassword"
            };
            var mockUser = new User()
            {
                Id = userId,
                Email = "unit@forex-miner.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = role,
                HasConnections = false
            };
            MockUserContext(userId, role);
            _userServiceMock.Setup(us => us.UpdateUserById(userId, userUpdate)).Returns(mockUser);

            // Act
            var user = _userController.UpdateUser(userId, userUpdate);

            // Assert
            _userServiceMock.Verify(us => us.UpdateUserById(userId, userUpdate), Times.Once());
            Assert.Equal(mockUser, user);
        }

        [Fact]
        public void UpdateUser_Admin()
        {
            // Arrange
            var requesterUserId = Guid.NewGuid();
            var requesterRole = Role.Admin;
            var userId = Guid.NewGuid();
            var role = Role.Trader;
            var userUpdate = new UserUpdate()
            {
                Email = "unit@forex-miner.ai",
                FirstName = "Unit",
                LastName = "Test",
                Password = "IWantANewPassword"
            };
            var mockUser = new User()
            {
                Id = userId,
                Email = "unit@forex-miner.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = role,
                HasConnections = false
            };
            MockUserContext(requesterUserId, requesterRole);
            _userServiceMock.Setup(us => us.UpdateUserById(userId, userUpdate)).Returns(mockUser);

            // Act
            var user = _userController.UpdateUser(userId, userUpdate);

            // Assert
            _userServiceMock.Verify(us => us.UpdateUserById(userId, userUpdate), Times.Once());
            Assert.Equal(mockUser, user);
        }

        [Fact]
        public void UpdateUser_DifferentUserId()
        {
            // Arrange
            var requesterUserId = Guid.NewGuid();
            var requesterRole = Role.Trader;
            var userId = Guid.NewGuid();
            var role = Role.Trader;
            var userUpdate = new UserUpdate()
            {
                Email = "unit@forex-miner.ai",
                FirstName = "Unit",
                LastName = "Test",
                Password = "IWantANewPassword"
            };
            var mockUser = new User()
            {
                Id = userId,
                Email = "unit@forex-miner.ai",
                FirstName = "Unit",
                LastName = "Test",
                Role = role,
                HasConnections = false
            };
            MockUserContext(requesterUserId, requesterRole);
            _userServiceMock.Setup(us => us.UpdateUserById(userId, userUpdate)).Returns(mockUser);

            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _userController.UpdateUser(userId, userUpdate));
            Assert.Equal(HttpStatusCode.Forbidden, exception.Status);
            _userServiceMock.Verify(us => us.UpdateUserById(userId, userUpdate), Times.Never());
        }

        [Fact]
        public void DeleteUser_SameUserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = Role.Trader;
            MockUserContext(userId, role);
            _userServiceMock.Setup(us => us.DeleteUserById(userId));

            // Act
            _userController.DeleteUser(userId);

            // Assert
            _userServiceMock.Verify(us => us.DeleteUserById(userId), Times.Once());
        }

        [Fact]
        public void DeleteUser_Admin()
        {
            // Arrange
            var requesterUserId = Guid.NewGuid();
            var requesterRole = Role.Admin;
            var userId = Guid.NewGuid();
            MockUserContext(requesterUserId, requesterRole);
            _userServiceMock.Setup(us => us.DeleteUserById(userId));

            // Act
            _userController.DeleteUser(userId);

            // Assert
            _userServiceMock.Verify(us => us.DeleteUserById(userId), Times.Once());
        }

        [Fact]
        public void DeleteUser_DifferentUserId()
        {
            // Arrange
            var requesterUserId = Guid.NewGuid();
            var requesterRole = Role.Trader;
            var userId = Guid.NewGuid();
            MockUserContext(requesterUserId, requesterRole);
            _userServiceMock.Setup(us => us.DeleteUserById(userId));

            // Act
            // Assert
            var exception = Assert.Throws<ProblemDetailsException>(() => _userController.DeleteUser(userId));
            Assert.Equal(HttpStatusCode.Forbidden, exception.Status);
            _userServiceMock.Verify(us => us.DeleteUserById(It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public void GetUsers()
        {
            // Arrange
            var mockUsers = new List<User>()
            {
                new User()
                {
                    Id = Guid.NewGuid(),
                    Email = "unit@test.ai",
                    FirstName = "Unit",
                    LastName = "Test",
                    Role = Role.Trader,
                    HasConnections = false
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    Email = "unit2@test.ai",
                    FirstName = "Unit2",
                    LastName = "Test",
                    Role = Role.Trader,
                    HasConnections = true
                }
            };
            _userServiceMock.Setup(us => us.GetAllUsers()).Returns(mockUsers);

            // Act
            var users = _userController.GetAllUsers();

            // Assert
            _userServiceMock.Verify(us => us.GetAllUsers(), Times.Once());
            Assert.Equal(mockUsers, users);
        }

        private void MockUserContext(Guid userId, Role role)
        {
            // Generate claims
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role.ToString())
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Mock base http context
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(ctx => ctx.User).Returns(claimsPrincipal);

            // Mock controller context
            var controllerContext = new ControllerContext()
            {
                HttpContext = contextMock.Object
            };
            _userController.ControllerContext = controllerContext;
        }
    }
}
