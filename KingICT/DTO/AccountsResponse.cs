using System;
using KingICT.Models;
using System.Text.Json.Serialization;

namespace KingICT.DTO
{
	public class AccountsResponse
	{
        [JsonPropertyName("users")]
        public List<Accounts> Users { get; set; }
    }
}

