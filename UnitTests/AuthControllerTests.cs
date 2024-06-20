using System;
using System.Linq;
using System.Net.Http;
using FakeItEasy;
using KingICT.Controllers;
using KingICT.DTO;
using KingICT.Models;
using KingICT.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
	public class AuthControllerTests
	{
        private readonly IAuthRepository _fakeRepo;
		private readonly AuthController _authController;
        private readonly IJwtService _fakeJwtService;
        private readonly ITokenBlacklistService _fakeTokenBlacklistService;

        public AuthControllerTests()
		{
            _fakeRepo = A.Fake<IAuthRepository>();
			_fakeJwtService = A.Fake<IJwtService>();
			_fakeTokenBlacklistService = A.Fake<ITokenBlacklistService>();
			_authController = new AuthController(_fakeRepo, _fakeJwtService, _fakeTokenBlacklistService);

            var context = new DefaultHttpContext();
            _authController.ControllerContext = new ControllerContext()
            {
                HttpContext = context
            };
        }
		
        [Fact]
		public async Task GetAccounts_WhenAccountsExist_ReturnsListOfAccounts()
		{
			// Arrange
			var fakeAccounts = new List<Accounts>
			{
				new Accounts {Username = "acc1", Password = "pass1"},
				new Accounts {Username = "acc2", Password = "pass2"}
			};
            A.CallTo(() => _fakeRepo.GetAccounts()).Returns(Task.FromResult(fakeAccounts));

			// Act
			var result = await _authController.GetAccounts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Accounts>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Accounts>>(okResult.Value);

            Assert.Equal(fakeAccounts.Count, returnValue.Count);
            for (int i = 0; i < fakeAccounts.Count; i++)
            {
                Assert.Equal(fakeAccounts[i].Username, returnValue[i].Username);
                Assert.Equal(fakeAccounts[i].Password, returnValue[i].Password);
            }
        }

        [Fact]
        public async Task GetAccounts_WhenAccountsAreNull_ReturnsNotFound()
        {
            // Arrange
            A.CallTo(() => _fakeRepo.GetAccounts()).Returns(Task.FromResult<List<Accounts>>(null));

            // Act
            var result = await _authController.GetAccounts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Accounts>>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetAccounts_WhenAccountsAreEmpty_ReturnsEmptyList()
        {
            // Arrange
            var fakeAccounts = new List<Accounts>
            {
            };
            A.CallTo(() => _fakeRepo.GetAccounts()).Returns(Task.FromResult(fakeAccounts));

            // Act
            var result = await _authController.GetAccounts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<Accounts>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<Accounts>>(okResult.Value);

            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task Login_WhenAccountIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await _authController.Login(null);

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Username or password not provided!", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_WhenUsernameIsMissing_ReturnsUnauthorized()
        {
            // Arrange
            var account = new Accounts { Username = "", Password = "pass1" };

            // Act
            var result = await _authController.Login(account);

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var unaothorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            Assert.Equal("Invalid credentials", unaothorizedResult.Value);
        }

        [Fact]
        public async Task Login_WhenPasswordIsMissing_ReturnsUnauthorized()
        {
            // Arrange
            var account = new Accounts { Username = "acc1", Password = "" };

            // Act
            var result = await _authController.Login(account);

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var unaothorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            Assert.Equal("Invalid credentials", unaothorizedResult.Value);
        }

        [Fact]
        public async Task Login_WhenCredentialsAreInvalid_ReturnsUnauthorized()
        {
            // Arrange
            var account = new Accounts { Username = "user", Password = "wrongpassword" };
            A.CallTo(() => _fakeRepo.Login(account)).Throws<UnauthorizedAccessException>();

            // Act
            var result = await _authController.Login(account);

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_WhenAccountIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var account = new Accounts { Username = "user", Password = "pass" };
            A.CallTo(() => _fakeRepo.Login(account)).Returns(Task.FromResult<AccountsDTO>(null));

            // Act
            var result = await _authController.Login(account);

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal("Account not found!", notFoundResult.Value);
        }

        [Fact]
        public async Task Login_WhenJwtGenerationFails_Returns500()
        {
            // Arrange
            var account = new Accounts { Username = "emilys", Password = "emilyspass" };
            var accountsDTO = new AccountsDTO
            {
                Id = 1,
                Email = "emilys@email.com",
                FirstName = "Emilys",
                LastName = "EmilysLastName",
                Gender = "Female",
                Image = "sample-image.com",
                Token = "sample-token",
                RefreshToken = "sample-refresh-token"
            };
            A.CallTo(() => _fakeRepo.Login(account)).Returns(Task.FromResult(accountsDTO));
            A.CallTo(() => _fakeJwtService.GenerateJWT(accountsDTO)).Returns("");

            // Act
            var result = await _authController.Login(account);

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var serverErrorResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            Assert.Equal("Error while generating JWT", serverErrorResult.Value);
        }

        [Fact]
        public async Task Login_WhenLoginIsSuccessful_ReturnsOk()
        {
            // Arrange
            var account = new Accounts { Username = "emilys", Password = "emilyspass" };
            var accountsDTO = new AccountsDTO
            {
                Id = 1,
                Email = "emilys@email.com",
                FirstName = "Emilys",
                LastName = "EmilysLastName",
                Gender = "Female",
                Image = "sample-image.com",
                Token = "sample-token",
                RefreshToken = "sample-refresh-token"
            };
            var validJwt = "valid-jwt";
            A.CallTo(() => _fakeRepo.Login(account)).Returns(Task.FromResult(accountsDTO));
            A.CallTo(() => _fakeJwtService.GenerateJWT(accountsDTO)).Returns(validJwt);

            // Act
            var result = await _authController.Login(account);

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var setCookieHeader = _authController.HttpContext.Response.Headers["Set-Cookie"].ToString();

            Assert.Contains("jwt", setCookieHeader);
            Assert.Contains(validJwt, setCookieHeader);
            Assert.Equal("Login Successful!", okResult.Value);
        }

        [Fact]
        public void Logout_WhenJwtCookieIsNotPresent_ReturnsUnauthorized()
        {
            // Act
            var result = _authController.Logout();

            // Assert
            A.CallTo(() => _fakeTokenBlacklistService.BlacklistToken(A<string>.Ignored)).MustNotHaveHappened();

            var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Must be logged in to use this feature!", actionResult.Value);
        }

        [Fact]
        public void Logout_WhenJwtCookieExists_ReturnsOk()
        {
            // Arrange
            var fakeHttpContext = A.Fake<HttpContext>();
            var fakeHttpRequest = A.Fake<HttpRequest>();
            var fakeCookies = A.Fake<IRequestCookieCollection>();

            var blacklistedTokens = new List<string>();

            A.CallTo(() => fakeCookies.ContainsKey(A<string>.Ignored)).Returns(true);
            A.CallTo(() => fakeCookies[A<string>.Ignored]).Returns("valid-jwt");
            A.CallTo(() => fakeHttpRequest.Cookies).Returns(fakeCookies);
            A.CallTo(() => fakeHttpContext.Request).Returns(fakeHttpRequest);

            A.CallTo(() => _fakeTokenBlacklistService.BlacklistToken(A<string>.Ignored))
                .Invokes((string token) => blacklistedTokens.Add(token));

            A.CallTo(() => _fakeTokenBlacklistService.isTokenBlacklisted(A<string>.Ignored))
                .ReturnsLazily((string token) => blacklistedTokens.Contains(token));

            _authController.ControllerContext.HttpContext = fakeHttpContext;

            // Act
            var result = _authController.Logout();
            var blacklistResult = _fakeTokenBlacklistService.isTokenBlacklisted("valid-jwt");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(blacklistResult);
            Assert.Equal("Logout Successful!", okResult.Value);
        }
    }
}

