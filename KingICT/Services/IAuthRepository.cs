using System;
using KingICT.DTO;
using KingICT.Models;

namespace KingICT.Services
{
	public interface IAuthRepository
	{
		Task<List<AccountsDBO>> GetAccounts();
		Task<AccountsDTO> Login(AccountsDBO account);
		Task<AccountsDTO> GetCurrentAccount(string authToken);
	}
}

