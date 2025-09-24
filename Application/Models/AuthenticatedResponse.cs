namespace Application.Models
{
    public class AuthenticatedResponse
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
