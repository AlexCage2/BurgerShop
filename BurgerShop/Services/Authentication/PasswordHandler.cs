using System.Security.Cryptography;
using System.Text;
namespace BurgerShop.Services.Authentication
{
    public sealed class PasswordHandler : IPasswordHandler
    {
        public bool TryCreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            try
            {
                using (var hmac = new HMACSHA512())
                {
                    passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                }
            }
            catch (Exception)
            {
                passwordHash = null;
                passwordSalt= null;
                return false;
            }
                
            return true; 
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {            
            using (var hmac = new HMACSHA512(passwordSalt))
            { 
                var computedHash = hmac.ComputeHash(Encoding
                    .UTF8
                    .GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }            
        }
    }
}
