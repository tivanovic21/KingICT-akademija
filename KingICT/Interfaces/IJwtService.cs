using System;
using KingICT.DTO;

namespace KingICT.Services
{
	public interface IJwtService
	{
		string GenerateJWT(AccountsDTO loggedUser);
		bool ValidateToken(string token);
    }
}

