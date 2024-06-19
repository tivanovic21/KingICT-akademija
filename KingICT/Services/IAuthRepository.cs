using System;
using KingICT.Models;

namespace KingICT.Services
{
	public interface IAuthRepository
	{
		Task<Accounts> GetAccount();
	}
}

