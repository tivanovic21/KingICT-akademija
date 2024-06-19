using System;
using KingICT.DTO;
using KingICT.Models;

namespace KingICT.Services
{
	public interface IAuthRepository
	{
		Task<List<Accounts>> GetAccounts();
		Task<string> Login(Accounts account);
		Task<AccountsDTO> GetCurrentAccount(string authToken);
	}
}

