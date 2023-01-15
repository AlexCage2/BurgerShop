using BurgerShop.Models.ApplicationModels.Sales;
using BurgerShop.Models.DataModels.Orders;
using Npgsql;

namespace BurgerShop.Data.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly string _connectionString;

        public PurchaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreatePurchaseAsync(Order order)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                foreach (var menuItem in order.Purchases)
                {
                    string sqlQuery = @"
                        INSERT INTO purchases (menu_item_id, order_id, amount)
                        VALUES
                        (@menu_item_id, @order_id, @amount)
                        ;";

                    NpgsqlCommand sqlCommand = new NpgsqlCommand(sqlQuery, sqlConnection);

                    sqlCommand.Parameters.AddWithValue("@menu_item_id", menuItem.Key.Id);
                    sqlCommand.Parameters.AddWithValue("@order_id", order.Id);
                    sqlCommand.Parameters.AddWithValue("@amount", menuItem.Value);

                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<SaleItem>> GetSaleItemsListAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            List<SaleItem> result = new List<SaleItem>();
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    SELECT 
                        m.name,
                        p.amount,
                        m.price
                    FROM purchases AS p
                        JOIN menu_items AS m
                        ON p.menu_item_id  = m.menu_item_id
                    WHERE p.order_id = @orderId
                    ;";

                NpgsqlCommand sqlCommand = new NpgsqlCommand(sqlQuery, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@orderId", orderId);

                using (NpgsqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            SaleItem item = new SaleItem
                            {
                                Name = reader.GetString(0),
                                Amount = reader.GetInt32(1),
                                Price = reader.GetInt32(2)
                            };

                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<IEnumerable<SaleByItem>> GetSalesByItemsAsync(
            DateOnly startDate, 
            DateOnly endDate, 
            int linesPerPage, 
            int pageNumber, 
            string sortOrder, 
            CancellationToken cancellationToken = default)
        {
            List<SaleByItem> result = new List<SaleByItem>();
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    WITH subquery AS (
                        SELECT 
                            m.name,
                            m.price * SUM(p.amount) as profit,
                            ROW_NUMBER() OVER (ORDER BY m.name) AS num, 
                            COUNT(*) OVER() AS total
                        FROM purchases AS p 
                            JOIN menu_items AS m
                            ON p.menu_item_id = m.menu_item_id
                            JOIN orders AS o
                            ON o.order_id = p.order_id
                        WHERE order_date >= @startDate
                        AND order_date <= @endDate
                        GROUP BY m.name, m.price
                    )
                    SELECT * 
                    FROM subquery
                    ORDER BY ";

                    string sqlQuery2 = @"
                    LIMIT @limit
                    OFFSET @offset
                    ";

                string sqlOrder = "";
                switch (sortOrder)
                {
                    case "IndexDesc":
                        sqlOrder = "num DESC";
                        break;
                    case "NameAsc":
                        sqlOrder = "name ASC";
                        break;
                    case "NameDesc":
                        sqlOrder = "name DESC";
                        break;
                    case "SummAsc":
                        sqlOrder = "profit ASC";
                        break;
                    case "SummDesc":
                        sqlOrder = "profit DESC";
                        break;
                    default:
                        sqlOrder = "num ASC";
                        break;
                }

                sqlQuery += sqlOrder + sqlQuery2;

                NpgsqlCommand sqlCommand = new NpgsqlCommand(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@startDate", startDate);
                sqlCommand.Parameters.AddWithValue("@endDate", endDate);
                sqlCommand.Parameters.AddWithValue("@limit", linesPerPage);
                sqlCommand.Parameters.AddWithValue("@offset", linesPerPage * (pageNumber - 1));
                
                using (NpgsqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            int totalItems = reader.GetInt32(3);

                            SaleByItem item = new SaleByItem
                            {
                                Name = reader.GetString(0),
                                Profit = reader.GetInt32(1),
                                Id = reader.GetInt32(2),
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

        public async Task<IEnumerable<SaleByDay>> GetSalesByDaysAsync(
            DateOnly startDate,
            DateOnly endDate,
            int linesPerPage,
            int pageNumber, 
            string sortOrder, 
            CancellationToken cancellationToken = default)
        {
            List<SaleByDay> result = new List<SaleByDay>();
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    WITH subquery AS (
                        SELECT 
	                        ord_date,
	                        SUM(subQuery.itemSum) as profit,
	                        ROW_NUMBER() OVER () AS num, 
	                        COUNT(*) OVER() AS total
                        FROM (
	                        SELECT 
		                        o.order_date AS ord_date,
		                        m.price * SUM(p.amount) as itemSum		
	                        FROM purchases AS p 
		                        JOIN menu_items AS m
		                        ON p.menu_item_id = m.menu_item_id
		                        JOIN orders AS o
		                        ON o.order_id = p.order_id
	                        WHERE order_date >= @startDate
	                        AND order_date <= @endDate
	                        GROUP BY o.order_date, m.price
                        ) AS subQuery
                        GROUP BY ord_date
                    )
                    SELECT * 
                    FROM subquery
                    ORDER BY ";

                string sqlQuery2 = @"
                    LIMIT @limit
                    OFFSET @offset
                    ";

                string sqlOrder = "";
                switch (sortOrder)
                {
                    case "IndexDesc":
                        sqlOrder = "num DESC";
                        break;
                    case "DateAsc":
                        sqlOrder = "ord_date ASC";
                        break;
                    case "DateDesc":
                        sqlOrder = "ord_date DESC";
                        break;
                    case "SummAsc":
                        sqlOrder = "profit ASC";
                        break;
                    case "SummDesc":
                        sqlOrder = "profit DESC";
                        break;
                    default:
                        sqlOrder = "num ASC";
                        break;
                }

                sqlQuery += sqlOrder + sqlQuery2;

                NpgsqlCommand sqlCommand = new NpgsqlCommand(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@startDate", startDate);
                sqlCommand.Parameters.AddWithValue("@endDate", endDate);
                sqlCommand.Parameters.AddWithValue("@limit", linesPerPage);
                sqlCommand.Parameters.AddWithValue("@offset", linesPerPage * (pageNumber - 1));

                using (NpgsqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            int totalItems = reader.GetInt32(3);

                            SaleByDay item = new SaleByDay
                            {
                                Date = DateOnly.FromDateTime(reader.GetDateTime(0)),
                                Profit = reader.GetInt32(1),
                                Id = reader.GetInt32(2),
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
