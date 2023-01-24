using BurgerShop.Models.ApplicationModels.Sales;
using BurgerShop.Models.DataModels.Orders;
using Npgsql;

namespace BurgerShop.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString; 

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreateOrderAsync(Order order)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    INSERT INTO orders (order_id, order_date, user_id)
                    VALUES
                    (@id, @date, @user_id)
                    ;";

                NpgsqlCommand sqlCommand = new NpgsqlCommand(sqlQuery, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@id", order.Id);
                sqlCommand.Parameters.AddWithValue("@date", order.Order_Date);
                sqlCommand.Parameters.AddWithValue("@user_id", order.UserId);

                await sqlCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<SaleInfo> GetSaleInfoAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    SELECT 
                        o.order_id,
                        o.order_date,
                        u.login
                    FROM orders AS o
                        JOIN users AS u
                        ON o.user_id = u.user_id
                    WHERE o.order_id = @orderId
                    LIMIT 1
                    ;";

                NpgsqlCommand sqlCommand = new NpgsqlCommand(sqlQuery, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@orderId", orderId);

                using (NpgsqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            return new SaleInfo
                            {
                                OrderId = reader.GetGuid(0),
                                OrderDate = DateOnly.FromDateTime(reader.GetDateTime(1)),
                                UserName = reader.GetString(2)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<IEnumerable<Sale>> GetItemsAsync(
            DateOnly startDate,
            DateOnly endDate,
            string userName,
            int linesPerPage,
            int pageNumber,
            string sortOrder,
            CancellationToken cancellationToken = default)
        {
            List<Sale> result = new List<Sale>();
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    WITH subquery AS (
                        SELECT 
                            o.order_id,
                            o.order_date,
                            u.login,
                            ROW_NUMBER() OVER () AS num, 
                            COUNT(*) OVER() AS total
                        FROM orders AS o
                            JOIN users AS u
                            ON u.user_id = o.user_id
                        WHERE order_date >= @startDate
                        AND order_date <= @endDate
                        AND login LIKE @userName
                    )
                    SELECT * 
                    FROM subquery
                    ";

                string sqlOrder = "";
                switch (sortOrder)
                {
                    case "DateAsc":
                        sqlOrder = "ORDER BY order_date ASC";
                        break;
                    case "IdAsc":
                        sqlOrder = "ORDER BY order_id ASC";
                        break;
                    case "IdDesc":
                        sqlOrder = "ORDER BY order_id DESC";
                        break;
                    case "UserAsc":
                        sqlOrder = "ORDER BY login ASC";
                        break;
                    case "UserDesc":
                        sqlOrder = "ORDER BY login DESC";
                        break;
                    default:
                        sqlOrder = "ORDER BY order_date DESC";
                        break;
                }

                string sqlQuery2 = @"
                    LIMIT @limit
                    OFFSET @offset
                    ";

                sqlQuery += sqlOrder + sqlQuery2;

                NpgsqlCommand sqlCommand = new NpgsqlCommand(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@startDate", startDate);
                sqlCommand.Parameters.AddWithValue("@endDate", endDate);
                sqlCommand.Parameters.AddWithValue("@limit", linesPerPage);
                sqlCommand.Parameters.AddWithValue("@offset", linesPerPage * (pageNumber - 1));
                sqlCommand.Parameters.AddWithValue("@userName", '%' + userName.Trim().ToLower() + '%');

                using (NpgsqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            int totalItems = reader.GetInt32(4);

                            Sale item = new Sale
                            {
                                OrderId = reader.GetGuid(0),
                                Order_Date = DateOnly.FromDateTime(reader.GetDateTime(1)),
                                UserName = reader.GetString(2),
                                PageNumber = pageNumber,
                                NumberOfPages = totalItems % linesPerPage == 0 ? (totalItems / linesPerPage) : (totalItems / linesPerPage + 1)
                            };

                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }
    }
}
