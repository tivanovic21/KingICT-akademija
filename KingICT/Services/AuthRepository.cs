using System;
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

        public async Task<Accounts> GetAccount()
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

        public async Task<string> Login(Accounts account)
        {
            var body = new { account.Username, account.Password };
            var response = await _httpClient.PostAsJsonAsync("auth/login", body);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadFromJsonAsync<AccountsDTO>();
            var authToken = jsonResponse.Token;

            return authToken;
        }


        public async Task<AccountsDTO> GetCurrentAccount(string authToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "auth/me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            var accountResponse = JsonSerializer.Deserialize<AccountsDTO>(jsonResponse, options);
            return accountResponse;
        }
    }
}

