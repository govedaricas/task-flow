using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace task_flow_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly ITokenService _tokenService;

        public TokensController(ITaskFlowDbContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenApiModel tokenApiModel, CancellationToken cancellationToken)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");

            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity?.Name;
            var incomingHash = _tokenService.HashToken(refreshToken);

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);  

            if (user is null || user.RefreshTokenHash == null || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");

            if (!user.RefreshTokenHash.SequenceEqual(incomingHash))
                return BadRequest("Invalid client request");

            var newAccessToken = _tokenService.GenerateJWT(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenHash = _tokenService.HashToken(newRefreshToken);

            user.RefreshTokenHash = newRefreshTokenHash;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok(new AuthenticatedResponse
            {   
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke(CancellationToken cancellationToken)
        {
            var username = User.Identity?.Name;
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

            if (user == null) 
                return BadRequest();

            user.RefreshTokenHash = null;
            user.RefreshTokenExpiryTime = null;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
    }
}
