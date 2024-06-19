﻿using System;
using KingICT.DTO;
using KingICT.Models;

namespace KingICT.Services
{
	public interface IAuthRepository
	{
		Task<Accounts> GetAccount();
		Task<string> Login(Accounts account);
		Task<AccountsDTO> GetCurrentAccount(string authToken);
	}
}

