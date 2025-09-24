using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseModel>
    {
        private readonly ITokenService _tokenService;
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;

        public LoginCommandHandler(ITokenService tokenService, ITaskFlowDbContext dbContext, IPasswordHasher passwordHasher)
        {
            _tokenService = tokenService;
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponseModel> Handle (LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Username == request.Username)
                .FirstOrDefaultAsync(cancellationToken);    

            if (user == null)
            {
                throw new NotFoundException("User", "User not found.");
            }

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new ConflictException("User", "Invalid password.");
            }

            var accessToken = _tokenService.GenerateJWT(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _tokenService.HashToken(refreshToken);

            user.RefreshTokenHash = refreshTokenHash;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            _dbContext.CurrentUser = new UserIdentityDto
            {
                Id = user.Id,
                Username = user.Username
            };

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new LoginResponseModel{
                Success = true,
                Message = "Login Successfully",
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }

        public class UserIdentityDto : IUserIdentity
        {
            public int? Id { get; set; }
            public string? Username { get; set; }
        }
    }
}
