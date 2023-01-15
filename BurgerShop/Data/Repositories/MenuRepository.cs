using BurgerShop.Models.DataModels.ProductsAndDishes;
using Npgsql;

namespace BurgerShop.Data.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly string _connectionString;

        public MenuRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<MenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken = default)
        {
            var result = new List<MenuItem>();

            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    SELECT 
                        i.menu_item_id,
                        i.name,
                        t.name,
                        i.price
                    FROM menu_items AS i
                        JOIN menu_item_types AS t
                        ON i.menu_item_type_id = t.menu_item_type_id";

                NpgsqlCommand command = new NpgsqlCommand(sqlQuery, sqlConnection);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            MenuItem item = new MenuItem
                            {
                                Id = reader.GetGuid(0),
                                Name = reader.GetString(1),
                                MenuItemType = reader.GetString(2),
                                Price = reader.GetInt32(3)
                            };

                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<MenuItem> GetMenuItemAsync(string menuItemName, CancellationToken cancellationToken = default)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    SELECT 
                        i.menu_item_id,
                        t.name,
                        i.price
                    FROM menu_items AS i
                        JOIN menu_item_types AS t
                        ON t.menu_item_type_id = i.menu_item_type_id
                    WHERE i.name = @name
                    LIMIT 1
                    ;";

                NpgsqlCommand command = new NpgsqlCommand(sqlQuery, sqlConnection);
                command.Parameters.AddWithValue("@name", menuItemName);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows && !reader.IsClosed)
                    {
                        while (await reader.ReadAsync())
                        {
                            MenuItem menuItem = new MenuItem
                            {
                                Id = reader.GetGuid(0),
                                Name = menuItemName,
                                MenuItemType = reader.GetString(1),
                                Price = reader.GetInt32(2)
                            };

                            return menuItem;
                        }
                    }
                }
            }

            return null;
        }
    }
}