using System;
using KingICT.DTO;
using KingICT.Models;
using KingICT.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingICT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<Accounts>> GetAccount()
        {
            var account = await _authRepository.GetAccount();

            if (account == null) return NotFound();

            return Ok(account);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Login([FromBody] Accounts account)
        {
            if (account == null) return "Account does not exist!";
            if(string.IsNullOrEmpty(account.Username) || string.IsNullOrEmpty(account.Password))
            {
                return "Invalid credentials!";
            }

            try
            {
                var authToken = await _authRepository.Login(account);
                return authToken;
            }catch (UnauthorizedAccessException ex)
            {
                return Unauthorized("Invalid credentials");
            }catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [HttpGet("getCurrentAccount")]
        [Produces("application/json")]
        public async Task<ActionResult<AccountsDTO>> GetCurrentAccount([FromHeader] string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
            {
                throw new Exception("Authorization token not provided!");
            }

            try
            {
                var account = await _authRepository.GetCurrentAccount(authToken);
                return account;
            } catch(HttpRequestException ex)
            {
                return Unauthorized("Invalid authorization token provided!");
            }
        }
    }
}

