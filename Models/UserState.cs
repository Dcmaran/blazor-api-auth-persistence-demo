namespace BlazorAuthAPI.Models
{
    public class UserState
    {
        public string? Username { get; set; }
        public string? SessionToken { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
