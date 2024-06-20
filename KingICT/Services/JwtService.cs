using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KingICT.DTO;
using KingICT.Models;
using Microsoft.IdentityModel.Tokens;

namespace KingICT.Services
{
	public class JwtService : IJwtService
    {
		private readonly JwtSettings _jwtSettings;
		public JwtService(IConfiguration configuration)
		{
			_jwtSettings = new JwtSettings();
			configuration.GetSection("JwtSettings").Bind(_jwtSettings);
		}

		public string GenerateJWT(AccountsDTO loggedUser)
		{
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loggedUser.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: null, 
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
	}
}

