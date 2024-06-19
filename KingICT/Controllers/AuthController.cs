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
        public async Task<ActionResult<List<AccountsDBO>>> GetAccounts()
        {
            var accounts = await _authRepository.GetAccounts();

            if (accounts == null) return NotFound();

            return Ok(accounts);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Login([FromBody] AccountsDBO account)
        {
            if (account == null) BadRequest("Username or password not provided!");
            if(string.IsNullOrEmpty(account.Username) || string.IsNullOrEmpty(account.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            try
            {
                var loggedUser = await _authRepository.Login(account);
                if (loggedUser == null) return NotFound("Account not found!");

                var jwt = JwtService.GenerateJWT(loggedUser);
                if (string.IsNullOrEmpty(jwt)) return StatusCode(500, "Error while generating JWT");
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                Response.Cookies.Append("jwt", jwt, cookieOptions);

                return Ok("Login Successful!");
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

