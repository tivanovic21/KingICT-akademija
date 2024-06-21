using System;
using KingICT.DTO;
using KingICT.Models;

namespace KingICT.Services
{
	public interface IAuthRepository
	{
		Task<List<Accounts>> GetAccounts();
		Task<AccountsDTO> Login(Accounts account);
	}
}

