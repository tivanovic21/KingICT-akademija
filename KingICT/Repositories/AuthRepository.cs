﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using KingICT.Data;
using KingICT.DTO;
using KingICT.Models;

namespace KingICT.Services
{
    public class AuthRepository : IAuthRepository
	{
        private readonly ApplicationContext _context;
        private readonly HttpClient _httpClient;

        public AuthRepository(ApplicationContext context, HttpClient httpClient)
		{
            _context = context;
            _httpClient = httpClient;
		}

        public async Task<List<Accounts>> GetAccounts()
        {
            var response = await _httpClient.GetAsync("users");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true,
            };

            var accountResponse = JsonSerializer.Deserialize<AccountsResponse>(jsonResponse, options);
            return accountResponse.Users;
        }

        public async Task<AccountsDTO> Login(Accounts account)
        {
            var allAccounts = await GetAccounts();
            var selectedAccount = allAccounts.FirstOrDefault(u => u.Username == account.Username);
            if (selectedAccount == null) throw new ArgumentException("Account not found!");

            var body = new { selectedAccount.Username, account.Password };
            var response = await _httpClient.PostAsJsonAsync("auth/login", body);
            response.EnsureSuccessStatusCode();

            var accountReposnse = await response.Content.ReadFromJsonAsync<AccountsDTO>();
            return accountReposnse;
        }
    }
}

