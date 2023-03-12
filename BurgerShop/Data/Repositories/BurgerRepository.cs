using BurgerShop.Models.ApplicationModels.Burgers;
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

        public async Task<IEnumerable<BurgersListItem>> GetItemsAsync(
            string burgerName,
            int linesPerPage,
            int pageNumber,
            string sortOrder,
            CancellationToken cancellationToken = default)
        {
            IList<BurgersListItem> result = new List<BurgersListItem>();

            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    SELECT 
                        b.burger_id,
                        b.name,
                        b.price,
                        t.type,
                        ROW_NUMBER() OVER () AS num, 
                        COUNT(*) OVER() AS total
                    FROM 
                        burgers AS b
                        JOIN burger_types AS t
                        ON b.burger_type_id = t.burger_type_id
                    WHERE name LIKE @burgerName
                    ";

                string sqlOrder = "";
                switch (sortOrder)
                {
                    case "NameDesc":
                        sqlOrder = "ORDER BY name DESC";
                        break;
                    case "SummAsc":
                        sqlOrder = "ORDER BY price ASC";
                        break;
                    case "SummDesc":
                        sqlOrder = "ORDER BY price DESC";
                        break;
                    case "TypeAsc":
                        sqlOrder = "ORDER BY type ASC";
                        break;
                    case "TypeDesc":
                        sqlOrder = "ORDER BY type DESC";
                        break;
                    default:
                        sqlOrder = "ORDER BY name ASC";
                        break;
                }

                string sqlQuery2 = @"
                    LIMIT @limit
                    OFFSET @offset
                    ";

                sqlQuery += sqlOrder + sqlQuery2;

                NpgsqlCommand sqlCommand = new NpgsqlCommand(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@limit", linesPerPage);
                sqlCommand.Parameters.AddWithValue("@offset", linesPerPage * (pageNumber - 1));
                sqlCommand.Parameters.AddWithValue("@burgerName", $"%{burgerName.Trim().ToLower()}%");

                using (NpgsqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            int totalItems = reader.GetInt32(5);

                            var item = new BurgersListItem
                            {
                                Id = reader.GetGuid(0),
                                Name = reader.GetString(1),
                                Price = reader.GetInt32(2),
                                BurgerType = reader.GetString(3),
                                PageNumber = pageNumber,
                                NumberOfPages = totalItems % linesPerPage == 0 ? (totalItems / linesPerPage) : (totalItems / linesPerPage + 1)
                            };

                            result.Add(item);
                        }
                    }
                }

                return result;
            }
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
