using System;
using FakeItEasy;
using KingICT.Controllers;
using KingICT.Models;
using KingICT.Services;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
	public class AuthControllerTests
	{
        private readonly IAuthRepository _fakeRepo;
		private readonly AuthController _authController;
        private readonly JwtService _fakeJwtService;
        private readonly ITokenBlacklistService _fakeTokenBlacklistService;

        public AuthControllerTests()
		{
            _fakeRepo = A.Fake<IAuthRepository>();
			_fakeJwtService = A.Fake<JwtService>();
			_fakeTokenBlacklistService = A.Fake<ITokenBlacklistService>();
			_authController = new AuthController(_fakeRepo, _fakeJwtService, _fakeTokenBlacklistService);
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

            Assert.Equal(fakeAccounts.Count, returnValue.Count);
        }
	}
}

