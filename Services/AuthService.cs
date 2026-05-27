using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagement.Services
{
    public class AuthService
    {
        private readonly PasswordHasher<AppUser> _passwordHasher = new();
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public AppUser? Authenticate(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Success)
            {
                return user;
            }
            return null;
        }

        public bool Register(string fullName, string email, string password)
        {
            if (_context.Users.Any(u => u.Email.ToLower() == email.ToLower()))
            {
                return false; // User exists
            }

            var user = new AppUser { Email = email, FullName = fullName, Role = "Librarian" };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            
            _context.Users.Add(user);
            _context.SaveChanges();
            
            return true;
        }
    }
}
