namespace AzureAppPizzeria.Data.Dtos.Authorization
{
    public class AuthResponseDto
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public IList<string>? Roles { get; set; }
    }
}
