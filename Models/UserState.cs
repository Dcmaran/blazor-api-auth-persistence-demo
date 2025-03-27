namespace BlazorAuthAPI.Models
{
    public class UserState
    {
        public string? Username { get; set; }
        public string? SessionID { get; set; }
        public bool IsAutenticated { get; set; }
    }
}
