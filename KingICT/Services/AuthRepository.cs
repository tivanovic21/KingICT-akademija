using System;
using System.Text.Json;
using KingICT.Data;
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

        public async Task<Accounts>GetAccount()
        {
            var response = await _httpClient.GetAsync("users/1");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true,
            };

            var accountResponse = JsonSerializer.Deserialize<Accounts>(jsonResponse, options);
            return accountResponse;
        }
    }
}

