// klasa statyczna do hashowania i odhashowania haseł

using System.Security.Cryptography;
using System.Text;

namespace projekt.Utils
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            string hashedInput = HashPassword(inputPassword);
            return hashedInput == storedHashedPassword;
        }
    }
}
