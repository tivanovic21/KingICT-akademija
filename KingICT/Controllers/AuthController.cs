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

        /// <summary>
        /// Retrieves a list of accounts that can be used for Login.
        /// </summary>
        /// <returns>A list of accounts if found, otherwise NotFound.</returns>
        /// <response code="200">Returns the list of accounts.</response>
        /// <response code="404">If no accounts are found.</response>
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<List<Accounts>>> GetAccounts()
        {
            var accounts = await _authRepository.GetAccounts();

            if (accounts == null || accounts.Count == 0) return NotFound();

            return Ok(accounts);
        }

        /// <summary>
        /// Logs in a user by validating their credentials and generating a JWT for them.
        /// </summary>
        /// <param name="account">The account object containing the username and password of the user.</param>
        /// <returns>A string indicating the result of the login operation.</returns>
        /// <response code="200">Returns the success message if login is successful.</response>
        /// <response code="400">If the account object is null or username or password is not provided.</response>
        /// <response code="401">If the credentials are invalid or unauthorized.</response>
        /// <response code="404">If the account is not found.</response>
        /// <response code="500">If there is an error while generating the JWT.</response>
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

        /// <summary>
        /// Logs out the user by validating their JWT (JSON Web Token), blacklisting it
        /// and removing it from cookies or the Authorization header.
        /// </summary>
        /// <returns>A string indicating the result of the logout operation.</returns>
        /// <response code="200">Returns the success message if logout is successful.</response>
        /// <response code="401">If the user is not logged in or the token is invalid.</response>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            string token = null;

            if (Request.Cookies.ContainsKey("jwt"))
            {
                token = Request.Cookies["jwt"];
            }
            else if (string.IsNullOrEmpty(token) && Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
            }

            if (!string.IsNullOrEmpty(token) && _jwtService.ValidateToken(token))
            {
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

