namespace Application.Features.Administration.Auth.Login
{
    public class LoginResponseModel
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}
