using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Settings;
using System.Text;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.BackgroundJobs
{
    public class HangfireDashboardJwtAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly JwtSettings _jwtSettings;

        public HangfireDashboardJwtAuthorizationFilter(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            string jwtToken = string.Empty;

            // 1. Get token from query string or cookie
            if (httpContext.Request.Query.ContainsKey("tkn"))
            {
                jwtToken = httpContext.Request.Query["tkn"].FirstOrDefault();

                // Save token in cookie for subsequent requests
                httpContext.Response.Cookies.Append(
                    "_hangfireCookie",
                    jwtToken,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // set true if using HTTPS
                        Expires = DateTime.UtcNow.AddMinutes(30)
                    });
            }
            else
            {
                jwtToken = httpContext.Request.Cookies["_hangfireCookie"];
            }

            if (string.IsNullOrEmpty(jwtToken))
                return false;

            try
            {
                // 2. Validate the token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = !string.IsNullOrEmpty(_jwtSettings.Issuer),
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = !string.IsNullOrEmpty(_jwtSettings.Audience),
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(jwtToken, validationParameters, out SecurityToken validatedToken);
                var jwtSecurityToken = (JwtSecurityToken)validatedToken;

                //TODO: When permissions added, allow only admin permission to access hangfire dashboard.

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
