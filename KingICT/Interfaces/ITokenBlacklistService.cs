using System;
namespace KingICT.Services
{
	public interface ITokenBlacklistService
	{
		void BlacklistToken(string token);
		bool isTokenBlacklisted(string token);
		HashSet<string> BlacklistedTokens();
	}
}

