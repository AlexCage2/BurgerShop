using BurgerShop.Models.DataModels.ProductsAndDishes;
using Npgsql;

namespace BurgerShop.Data.Repositories
{
    public class BurgerRepository : IBurgerRepository
    {
        private readonly string _connectionString;

        public BurgerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Burger> GetBurgerAsync(string burgerName, CancellationToken cancellationToken = default)
        {
            Burger burger = new();
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    SELECT 
                        b.burger_id,
                        b.name,
                        b.price,
                        t.type
                    FROM burgers AS b
                        JOIN burger_types AS t
                        ON b.burger_type_id = t.burger_type_id
                    WHERE b.name = @name
                    LIMIT 1
                    ";

                NpgsqlCommand command = new NpgsqlCommand(sqlQuery, sqlConnection);
                command.Parameters.AddWithValue("@name", burgerName);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows && !reader.IsClosed)
                    {
                        while (await reader.ReadAsync())
                        {
                            burger.Id = reader.GetGuid(0);
                            burger.Name = reader.GetString(1);
                            burger.Price = reader.GetInt32(2);
                            burger.BurgerType = reader.GetString(3);
                            burger.Recipe = new Dictionary<string, int>();
                        }
                    }
                }

                sqlQuery = @"
                    SELECT 
                        f.name,
                        r.amount
                    FROM recipes AS r
                        JOIN foods AS f
                        ON r.food_id = f.food_id
                    WHERE r.burger_id = @burgerid
                    ";

                command = new NpgsqlCommand(sqlQuery, sqlConnection);
                command.Parameters.AddWithValue("@burgerid", burger.Id);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows && !reader.IsClosed)
                    {
                        while (await reader.ReadAsync())
                        {
                            string productName = reader.GetString(0);
                            int productAmount = reader.GetInt32(1);
                            burger.Recipe.TryAdd(productName, productAmount);
                        }
                    }
                }
            }

            return burger;
        }
    }
}
