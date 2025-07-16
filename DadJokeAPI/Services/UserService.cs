using DadJokeAPI.Models;
using DadJokeConsoleApi.Data;

namespace DadJokeConsoleApi.Services
{
    public class UserService
    {
        private readonly DadJokeDbContext _context;

        public UserService(DadJokeDbContext context)
        {
            _context = context;
        }

        public bool Register(User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
                return false;

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public bool ValidateUser(string username, string password)
        {
            return _context.Users.Any(u => u.Username == username && u.Password == password);
        }
    }
}
