using System;
using KingICT.DTO;
using KingICT.Models;
using KingICT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KingICT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;
        private readonly ITokenBlacklistService _tokenBlacklistService;

        public AuthController(IAuthRepository authRepository, IJwtService jwtService, ITokenBlacklistService tokenBlacklistService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
            _tokenBlacklistService = tokenBlacklistService;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<List<Accounts>>> GetAccounts()
        {
            var accounts = await _authRepository.GetAccounts();

            if (accounts == null) return NotFound();

            return Ok(accounts);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Login([FromBody] Accounts account)
        {
            if (account == null) return BadRequest("Username or password not provided!");
            if(string.IsNullOrEmpty(account.Username) || string.IsNullOrEmpty(account.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            try
            {
                var loggedUser = await _authRepository.Login(account);
                if (loggedUser == null) return NotFound("Account not found!");

                var jwt = _jwtService.GenerateJWT(loggedUser);
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

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("jwt"))
            {
                var token = Request.Cookies["jwt"];
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(-1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                _tokenBlacklistService.BlacklistToken(token);
                Response.Cookies.Append("jwt", "", cookieOptions);
                return Ok("Logout Successful!");
            }
            return Unauthorized("Must be logged in to use this feature!");
        }
    }
}

