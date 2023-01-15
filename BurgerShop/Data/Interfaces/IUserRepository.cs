using BurgerShop.Models.DataModels.Users;

namespace BurgerShop.Data
{
    public interface IUserRepository
    {
        public Task<bool> TryCreateUserAsync(User user);
        public Task<User> GetUserAsync(string login, CancellationToken cancelationToken = default);
        public Task<bool> TryUpdateUserAsync(User user);
        public Task<bool> IsUserExistsAsync(string Login, CancellationToken cancellationToken = default);

    }
}
