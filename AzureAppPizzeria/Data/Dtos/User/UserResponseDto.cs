namespace AzureAppPizzeria.Data.Dtos.User
{
    public class UserResponseDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public int BonusPoints { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public List<int> OrderIds { get; set; } = new List<int>(); // <<< ÄNDRING HÄR

    }
}
