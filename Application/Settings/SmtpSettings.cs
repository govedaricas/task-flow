namespace Application.Settings
{
    public class SmtpSettings
    {
        public string Server { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
