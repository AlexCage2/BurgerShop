namespace BurgerShop.Data.Repositories
{
    public class RecipesRepository : IRecipesRepository
    {
        private readonly string _connectionString;

        public RecipesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
