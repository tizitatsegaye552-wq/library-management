namespace LibraryManagement.Models
{
    public class AppUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Librarian";
        public string FullName { get; set; } = string.Empty;
    }
}
