using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace task_flow.IntegrationTests.Auth;

public static class JwtTestHelper
{
    private const string SecretKey = "super-secret-key-for-integration-tests-minimum32chars!";
    private const string Issuer = "taskflow-test";
    private const string Audience = "taskflow-test-client";

    public static string GenerateExpiredToken()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: [new Claim(ClaimTypes.Name, "ghost"), new Claim(ClaimTypes.NameIdentifier, "999")],
            notBefore: DateTime.UtcNow.AddHours(-2),
            expires: DateTime.UtcNow.AddHours(-1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
