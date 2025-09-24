using System.Security.Claims;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        public string GenerateJWT(User user);
        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
        string GenerateRefreshToken();
        byte[] HashToken(string token);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
