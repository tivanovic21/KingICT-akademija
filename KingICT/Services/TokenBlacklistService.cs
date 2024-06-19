using System;
namespace KingICT.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
	{
        private readonly HashSet<string> _blacklistedTokens = new HashSet<string>();
		public TokenBlacklistService()
		{
		}

        public void BlacklistToken(string token)
        {
            _blacklistedTokens.Add(token);
        }

        public bool isTokenBlacklisted(string token)
        {
            return _blacklistedTokens.Contains(token);
        }
    }
}

