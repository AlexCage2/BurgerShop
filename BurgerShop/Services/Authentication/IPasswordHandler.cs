namespace BurgerShop.Services.Authentication
{
    public interface IPasswordHandler
    {
        public bool TryCreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}