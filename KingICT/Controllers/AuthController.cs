using System;
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
    }
}

