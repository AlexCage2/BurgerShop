using BurgerShop.Services.Authentication;
using Npgsql;

namespace BurgerShop.Data
{
    public static class DbInitializer
    {
        // Main Method
        public static async Task Initialize()
        {
            const string defaultConnectionString = "Server=localhost;Port=5432;User Id=postgres;Password=Ujewl4ki-1;Database=postgres;";
            string dbName = "burgersshopdb";

            await TryCreateDbAsync(defaultConnectionString, dbName);

            string connectionString = UpdateConnectionString(defaultConnectionString, dbName);

            await TryCreateTableUserRolesAsync(connectionString);
            await TryCreateTableUsersAsync(connectionString);

            await TryCreateTableProductTypesAsync(connectionString);
            await TryCreateTableProductsAsync(connectionString);
            await TryCreateTableMenuItemTypesAsync(connectionString);
            await TryCreateTableMenuItemsAsync(connectionString);
            await TryCreateTableBurgerTypesAsync(connectionString);
            await TryCreateTableBurgersAsync(connectionString);
            await TryCreateTableAlcoholicDrinksAsync(connectionString);
            await TryCreateTableNonAlcoholicDrinksAsync(connectionString);
            await TryCreateTableFoodsAsync(connectionString);
            await TryCreateTableRecipesAsync(connectionString);
            await TryCreateTableCitiesAsync(connectionString);
            await TryCreateTableSupplierStatusesAsync(connectionString);
            await TryCreateTableSuppliersAsync(connectionString);
            await TryCreateTableSupplieItemsAsync(connectionString);
            await TryCreateTableSuppliesAsync(connectionString);
            await TryCreateTableOrdersAsync(connectionString);
            await TryCreateTablePurchacesAsync(connectionString);
        }

        // Create DataBase
        public static async Task<bool> TryCreateDbAsync(string connectionString, string dbName)
        {
            try
            {
                string sqlQuery = $"create database {dbName};";

                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Create Table Roles
        public static async Task<bool> TryCreateTableUserRolesAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();         
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE user_roles (
                            role_id UUID PRIMARY KEY,
                            role    VARCHAR(10) NOT NULL
                        )";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO user_roles (role_id, role)
                        VALUES
                        (@id1, 'admin'),
                        (@id2, 'user');";

                    GenerateGuids(command, 1, 2);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }               

        // Create Table Users
        public static async Task<bool> TryCreateTableUsersAsync(string connectionString)
        {
            
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                PasswordHandler passwordHandler = new PasswordHandler();

                try
                {
                    command.CommandText = @"
                        CREATE TABLE users (
                            user_id         UUID PRIMARY KEY,
                            login           CHARACTER VARYING(30) NOT NULL,
                            passwordHash    BYTEA NOT NULL,
                            passwordSalt    BYTEA NOT NULL,
                            firstName       CHARACTER VARYING(30) NOT NULL,
                            lastName        CHARACTER VARYING(30) NOT NULL,
                            age             INTEGER NOT NULL,
                            role_id         UUID REFERENCES user_roles (role_id) ON UPDATE CASCADE ON DELETE CASCADE 
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO users (user_id, login, passwordHash, passwordSalt, firstName, lastName, age, role_id)
                        VALUES 
                        (@id1, 'admin',     @passHash1, @passSalt1, 'Bill',     'Gates', 67, (SELECT role_id FROM user_roles WHERE role = 'admin' LIMIT 1)),
                        (@id2, 'user',      @passHash2, @passSalt2, 'Nicolas',  'Cage',  58, (SELECT role_id FROM user_roles WHERE role = 'user'  LIMIT 1)),
                        (@id3, 'younguser', @passHash3, @passSalt3, 'El',       'Even',  11, (SELECT role_id FROM user_roles WHERE role = 'user'  LIMIT 1))
                        ;";

                    GenerateGuids(command, 1, 3);

                    passwordHandler.TryCreatePasswordHash("admin", out byte[] passHash, out byte[] passSalt);                    
                    command.Parameters.Add(new NpgsqlParameter("@passHash1", passHash));
                    command.Parameters.Add(new NpgsqlParameter("@passSalt1", passSalt));

                    passwordHandler.TryCreatePasswordHash("user", out passHash, out passSalt);
                    command.Parameters.Add(new NpgsqlParameter("@passHash2", passHash));
                    command.Parameters.Add(new NpgsqlParameter("@passSalt2", passSalt));

                    passwordHandler.TryCreatePasswordHash("younguser", out passHash, out passSalt);
                    command.Parameters.Add(new NpgsqlParameter("@passHash3", passHash));
                    command.Parameters.Add(new NpgsqlParameter("@passSalt3", passSalt));

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table ProductTypes
        public static async Task<bool> TryCreateTableProductTypesAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;  

                try
                {
                    command.CommandText = @"
                        CREATE TABLE product_types (
                            product_type_id UUID PRIMARY KEY,
                            name            VARCHAR(30) NOT NULL
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO product_types (product_type_id, name)
                        VALUES
                        (@id1, 'food'),
                        (@id2, 'drink'),
                        (@id3, 'alcohol');";

                    GenerateGuids(command, 1, 3);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table Products
        public static async Task<bool> TryCreateTableProductsAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE products (
                            product_id      UUID PRIMARY KEY,
                            name            VARCHAR(30) NOT NULL,
                            product_type_id UUID REFERENCES product_types (product_type_id)
                        );";

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table MenuItemTypes
        public static async Task<bool> TryCreateTableMenuItemTypesAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE menu_item_types (
                            menu_item_type_id   UUID PRIMARY KEY,
                            name                VARCHAR(30) NOT NULL
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO menu_item_types (menu_item_type_id, name)
                        VALUES
                        (@id1, 'burger'),
                        (@id2, 'drink'),
                        (@id3, 'alcohol');";

                    GenerateGuids(command, 1, 3);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table MenuItems
        public static async Task<bool> TryCreateTableMenuItemsAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE menu_items (
                            menu_item_id        UUID PRIMARY KEY,
                            name                VARCHAR(30) NOT NULL,
                            menu_item_type_id   UUID REFERENCES menu_item_types (menu_item_type_id),
                            price               NUMERIC NOT NULL DEFAULT 0
                        );";

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table BurgerTypes
        public static async Task<bool> TryCreateTableBurgerTypesAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE burger_types (
                            burger_type_id  UUID PRIMARY KEY,
                            type            VARCHAR(10) NOT NULL
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO burger_types (burger_type_id, type)
                        VALUES
                        (@id1, 'chicken'),
                        (@id2, 'meat'),
                        (@id3, 'fish');";

                    GenerateGuids(command, 1, 3);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table Burgers
        public static async Task<bool> TryCreateTableBurgersAsync(string connectionString)
        {            
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE burgers (
                            burger_id       UUID PRIMARY KEY,
                            name            CHARACTER VARYING(40) NOT NULL,
                            price           NUMERIC NOT NULL DEFAULT 0,
                            burger_type_id  UUID REFERENCES burger_types (burger_type_id)
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO burgers (burger_id, name, price, burger_type_id)
                        VALUES
                        (@id1, 'Hamburger',     69,  (SELECT burger_type_id FROM burger_types WHERE type = 'meat'    LIMIT 1)),
                        (@id2, 'Chickenburger', 59,  (SELECT burger_type_id FROM burger_types WHERE type = 'chicken' LIMIT 1)),
                        (@id3, 'Fishburger',    59,  (SELECT burger_type_id FROM burger_types WHERE type = 'fish'    LIMIT 1)),
                        (@id4, 'Cheese Jack',   149, (SELECT burger_type_id FROM burger_types WHERE type = 'meat'    LIMIT 1)),
                        (@id5, 'Rodeo Burger',  229, (SELECT burger_type_id FROM burger_types WHERE type = 'meat'    LIMIT 1)),
                        (@id6, 'Chicken Dream', 199, (SELECT burger_type_id FROM burger_types WHERE type = 'chicken' LIMIT 1)),
                        (@id7, 'BBQ Burger',    249, (SELECT burger_type_id FROM burger_types WHERE type = 'meat'    LIMIT 1)),
                        (@id8, 'Extra Burger',  329, (SELECT burger_type_id FROM burger_types WHERE type = 'meat'    LIMIT 1));";

                    GenerateGuids(command, 1, 8);

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO menu_items (menu_item_id, name, menu_item_type_id, price)
                        VALUES
                        (@id1, 'Hamburger',     (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'burger' LIMIT 1), 69),
                        (@id2, 'Chickenburger', (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'burger' LIMIT 1), 59),
                        (@id3, 'Fishburger',    (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'burger' LIMIT 1), 59),
                        (@id4, 'Cheese Jack',   (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'burger' LIMIT 1), 149),
                        (@id5, 'Rodeo Burger',  (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'burger' LIMIT 1), 229),
                        (@id6, 'Chicken Dream', (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'burger' LIMIT 1), 199),
                        (@id7, 'BBQ Burger',    (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'burger' LIMIT 1), 249),
                        (@id8, 'Extra Burger',  (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'burger' LIMIT 1), 329);";

                    command.Parameters.Clear();
                    GenerateGuids(command, 1, 8);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table NonAlcoholicDrinks
        public static async Task<bool> TryCreateTableNonAlcoholicDrinksAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE non_alcoholic_drinks (
                            non_alcoholic_drink_id  UUID PRIMARY KEY,
                            name                    VARCHAR(20) NOT NULL
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO non_alcoholic_drinks (non_alcoholic_drink_id, name)
                        VALUES
                        (@id1, 'Sprite'),
                        (@id2, 'Cola'),
                        (@id3, 'Fanta')
                        ;";

                    GenerateGuids(command, 1, 3);

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO menu_items (menu_item_id, name, menu_item_type_id, price)
                        VALUES
                        (@id1, 'Sprite', (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'drink' LIMIT 1), 99),
                        (@id2, 'Cola',   (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'drink' LIMIT 1), 99),
                        (@id3, 'Fanta',  (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'drink' LIMIT 1), 99)
                        ;";

                    command.Parameters.Clear();
                    GenerateGuids(command, 1, 3);

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO products (product_id, name, product_type_id)
                        VALUES
                        (@id1, 'Sprite', (SELECT product_type_id FROM product_types WHERE name = 'drink' LIMIT 1)),
                        (@id2, 'Cola',   (SELECT product_type_id FROM product_types WHERE name = 'drink' LIMIT 1)),
                        (@id3, 'Fanta',  (SELECT product_type_id FROM product_types WHERE name = 'drink' LIMIT 1))
                        ;";

                    command.Parameters.Clear();
                    GenerateGuids(command, 1, 3);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table AlcoholicDrinks
        public static async Task<bool> TryCreateTableAlcoholicDrinksAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE alcoholic_drinks (
                            alcoholic_drink_id UUID PRIMARY KEY,
                            name VARCHAR(20) NOT NULL
                        );";
                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO alcoholic_drinks (alcoholic_drink_id, name)
                        VALUES
                        (@id1, 'Whiskey'),
                        (@id2, 'Vodka'),
                        (@id3, 'Martini'),
                        (@id4, 'Vine'),
                        (@id5, 'Absinthe')
                        ;";

                    GenerateGuids(command, 1, 5);

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO menu_items (menu_item_id, name, menu_item_type_id, price)
                        VALUES
                        (@id1, 'Whiskey',  (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'alcohol' LIMIT 1), 399),
                        (@id2, 'Vodka',    (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'alcohol' LIMIT 1), 299),
                        (@id3, 'Martini',  (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'alcohol' LIMIT 1), 299),
                        (@id4, 'Vine',     (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'alcohol' LIMIT 1), 499),
                        (@id5, 'Absinthe', (SELECT menu_item_type_id FROM menu_item_types WHERE name = 'alcohol' LIMIT 1), 799)
                        ;";

                    command.Parameters.Clear();
                    GenerateGuids(command, 1, 5);

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO products (product_id, name, product_type_id)
                        VALUES
                        (@id1, 'Whiskey',  (SELECT product_type_id FROM product_types WHERE name = 'alcohol' LIMIT 1)),
                        (@id2, 'Vodka',    (SELECT product_type_id FROM product_types WHERE name = 'alcohol' LIMIT 1)),
                        (@id3, 'Martini',  (SELECT product_type_id FROM product_types WHERE name = 'alcohol' LIMIT 1)),
                        (@id4, 'Vine',     (SELECT product_type_id FROM product_types WHERE name = 'alcohol' LIMIT 1)),
                        (@id5, 'Absinthe', (SELECT product_type_id FROM product_types WHERE name = 'alcohol' LIMIT 1))
                        ;";

                    command.Parameters.Clear();
                    GenerateGuids(command, 1, 5);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table Foods
        public static async Task<bool> TryCreateTableFoodsAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE foods (
                            food_id UUID PRIMARY KEY,
                            name    VARCHAR(30) NOT NULL
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO foods (food_id, name)
                        VALUES
                        (@id1,  'tomato'),
                        (@id2,  'cucumber'),
                        (@id3,  'meat_cutlet'),
                        (@id4,  'fish_cutlet'),
                        (@id5,  'chicken_cutlet'),
                        (@id6,  'salad_pack'),
                        (@id7,  'fish_sause_pack'),
                        (@id8,  'chicken_sause_pack'),
                        (@id9,  'meat_sause_pack'),
                        (@id10, 'bbq_sause_pack'),
                        (@id11, 'cheese_sause_pack'),
                        (@id12, 'bread'),
                        (@id13, 'cheese_pack')
                        ;";

                    GenerateGuids(command, 1, 13);

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO products (product_id, name, product_type_id)
                        VALUES
                        (@id1,  'tomato',             (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id2,  'cucumber',           (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id3,  'meat_cutlet',        (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id4,  'fish_cutlet',        (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id5,  'chicken_cutlet',     (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id6,  'salad_pack',         (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id7,  'fish_sause_pack',    (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id8,  'chicken_sause_pack', (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id9,  'meat_sause_pack',    (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id10, 'bbq_sause_pack',     (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id11, 'cheese_sause_pack',  (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id12, 'bread',              (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1)),
                        (@id13, 'cheese_pack',        (SELECT product_type_id FROM product_types WHERE name = 'food' LIMIT 1))
                        ;";

                    command.Parameters.Clear();
                    GenerateGuids(command, 1, 13);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table Recipes
        public static async Task<bool> TryCreateTableRecipesAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE recipes (
                            burger_id   UUID REFERENCES burgers (burger_id) ON UPDATE CASCADE ON DELETE CASCADE,
                            food_id     UUID REFERENCES foods (food_id) ON UPDATE CASCADE,
                            amount      NUMERIC NOT NULL DEFAULT 1,
                            CONSTRAINT  recipes_pkey PRIMARY KEY (burger_id, food_id)
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO recipes (burger_id, food_id, amount)
                        VALUES
                        ((SELECT burger_id FROM burgers WHERE name = 'Hamburger'     LIMIT 1), (SELECT food_id FROM foods WHERE name = 'meat_cutlet'        LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Hamburger'     LIMIT 1), (SELECT food_id FROM foods WHERE name = 'bread'              LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Chickenburger' LIMIT 1), (SELECT food_id FROM foods WHERE name = 'chicken_cutlet'     LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Chickenburger' LIMIT 1), (SELECT food_id FROM foods WHERE name = 'bread'              LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Fishburger'    LIMIT 1), (SELECT food_id FROM foods WHERE name = 'fish_cutlet'        LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Fishburger'    LIMIT 1), (SELECT food_id FROM foods WHERE name = 'bread'              LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Cheese Jack'   LIMIT 1), (SELECT food_id FROM foods WHERE name = 'meat_cutlet'        LIMIT 1), 2),
                        ((SELECT burger_id FROM burgers WHERE name = 'Cheese Jack'   LIMIT 1), (SELECT food_id FROM foods WHERE name = 'bread'              LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Cheese Jack'   LIMIT 1), (SELECT food_id FROM foods WHERE name = 'cheese_sause_pack'  LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Cheese Jack'   LIMIT 1), (SELECT food_id FROM foods WHERE name = 'cheese_pack'        LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Rodeo Burger'  LIMIT 1), (SELECT food_id FROM foods WHERE name = 'meat_cutlet'        LIMIT 1), 2),
                        ((SELECT burger_id FROM burgers WHERE name = 'Rodeo Burger'  LIMIT 1), (SELECT food_id FROM foods WHERE name = 'bread'              LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Rodeo Burger'  LIMIT 1), (SELECT food_id FROM foods WHERE name = 'meat_sause_pack'    LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Rodeo Burger'  LIMIT 1), (SELECT food_id FROM foods WHERE name = 'tomato'             LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Chicken Dream' LIMIT 1), (SELECT food_id FROM foods WHERE name = 'chicken_cutlet'     LIMIT 1), 3),
                        ((SELECT burger_id FROM burgers WHERE name = 'Chicken Dream' LIMIT 1), (SELECT food_id FROM foods WHERE name = 'bread'              LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Chicken Dream' LIMIT 1), (SELECT food_id FROM foods WHERE name = 'chicken_sause_pack' LIMIT 1), 2),
                        ((SELECT burger_id FROM burgers WHERE name = 'Chicken Dream' LIMIT 1), (SELECT food_id FROM foods WHERE name = 'salad_pack'         LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'BBQ Burger'    LIMIT 1), (SELECT food_id FROM foods WHERE name = 'meat_cutlet'        LIMIT 1), 2),
                        ((SELECT burger_id FROM burgers WHERE name = 'BBQ Burger'    LIMIT 1), (SELECT food_id FROM foods WHERE name = 'bread'              LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'BBQ Burger'    LIMIT 1), (SELECT food_id FROM foods WHERE name = 'bbq_sause_pack'     LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Extra Burger'  LIMIT 1), (SELECT food_id FROM foods WHERE name = 'meat_cutlet'        LIMIT 1), 3),
                        ((SELECT burger_id FROM burgers WHERE name = 'Extra Burger'  LIMIT 1), (SELECT food_id FROM foods WHERE name = 'bread'              LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Extra Burger'  LIMIT 1), (SELECT food_id FROM foods WHERE name = 'meat_sause_pack'    LIMIT 1), 2),
                        ((SELECT burger_id FROM burgers WHERE name = 'Extra Burger'  LIMIT 1), (SELECT food_id FROM foods WHERE name = 'cucumber'           LIMIT 1), 1),
                        ((SELECT burger_id FROM burgers WHERE name = 'Extra Burger'  LIMIT 1), (SELECT food_id FROM foods WHERE name = 'tomato'             LIMIT 1), 2),
                        ((SELECT burger_id FROM burgers WHERE name = 'Extra Burger'  LIMIT 1), (SELECT food_id FROM foods WHERE name = 'salad_pack'         LIMIT 1), 1)
                        ;"; 

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table Cities
        public static async Task<bool> TryCreateTableCitiesAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE cities (
                            city_id UUID PRIMARY KEY,
                            name    VARCHAR(30) NOT NULL
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO cities (city_id, name)
                        VALUES
                        (@id1, 'Moscow'),
                        (@id2, 'Saint-Petersburg'),
                        (@id3, 'Krasnodar'),
                        (@id4, 'Samara'),
                        (@id5, 'Khabarovsk');";

                    GenerateGuids(command, 1, 5);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table SupplierStatuses
        public static async Task<bool> TryCreateTableSupplierStatusesAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE supplier_statuses (
                            supplier_status_id  UUID PRIMARY KEY,
                            name                VARCHAR(30) NOT NULL
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO supplier_statuses (supplier_status_id, name)
                        VALUES
                        (@id1, 'Active'),
                        (@id2, 'Testing'),
                        (@id3, 'Blacklist'),
                        (@id4, 'Waiting');";

                    GenerateGuids(command, 1, 4);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table Suppliers
        public static async Task<bool> TryCreateTableSuppliersAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE suppliers (
                            supplier_id         UUID PRIMARY KEY,
                            name                VARCHAR(30) NOT NULL,
                            supplier_status_id  UUID REFERENCES supplier_statuses (supplier_status_id) ON UPDATE CASCADE,
                            city_id             UUID REFERENCES cities (city_id) ON UPDATE CASCADE,
                            address             TEXT NOT NULL,
                            phone               VARCHAR(30) NOT NULL,
                            site                TEXT,
                            information         TEXT
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO suppliers (supplier_id, name, supplier_status_id, city_id, address, phone)
                        VALUES
                        (@id1, 'FreshMeat',           (SELECT supplier_status_id FROM supplier_statuses WHERE name = 'Active'    LIMIT 1), (SELECT city_id FROM cities WHERE name = 'Krasnodar'        LIMIT 1), 'Krasnaya 12',           '8-800-123-45-67'),
                        (@id2, 'FreshVegetable',      (SELECT supplier_status_id FROM supplier_statuses WHERE name = 'Active'    LIMIT 1), (SELECT city_id FROM cities WHERE name = 'Krasnodar'        LIMIT 1), 'Sinyaya 34',            '8-800-234-56-78'),
                        (@id3, 'KhabarovskFish',      (SELECT supplier_status_id FROM supplier_statuses WHERE name = 'Testing'   LIMIT 1), (SELECT city_id FROM cities WHERE name = 'Khabarovsk'       LIMIT 1), 'Lenina 1',              '8-800-345-67-89'),
                        (@id4, 'Kolosok',             (SELECT supplier_status_id FROM supplier_statuses WHERE name = 'Active'    LIMIT 1), (SELECT city_id FROM cities WHERE name = 'Krasnodar'        LIMIT 1), 'Belaya 56',             '8-800-456-78-90'),
                        (@id5, 'SaintPetersburgFish', (SELECT supplier_status_id FROM supplier_statuses WHERE name = 'Waiting'   LIMIT 1), (SELECT city_id FROM cities WHERE name = 'Saint-Petersburg' LIMIT 1), 'Nevskii pr 184',        '8-800-567-89-01'),
                        (@id6, 'ChickenFarm',         (SELECT supplier_status_id FROM supplier_statuses WHERE name = 'Active'    LIMIT 1), (SELECT city_id FROM cities WHERE name = 'Samara'           LIMIT 1), 'Mira 74',               '8-800-678-90-12'),
                        (@id7, 'SauseMaster',         (SELECT supplier_status_id FROM supplier_statuses WHERE name = 'Active'    LIMIT 1), (SELECT city_id FROM cities WHERE name = 'Moscow'           LIMIT 1), 'Mira 13',               '8-800-789-01-23'),
                        (@id8, 'EduardStore',         (SELECT supplier_status_id FROM supplier_statuses WHERE name = 'Blacklist' LIMIT 1), (SELECT city_id FROM cities WHERE name = 'Moscow'           LIMIT 1), 'Mira str Kefira house', '8-800-890-12-34'),
                        (@id9, 'AlcoMarket',          (SELECT supplier_status_id FROM supplier_statuses WHERE name = 'Active'    LIMIT 1), (SELECT city_id FROM cities WHERE name = 'Moscow'           LIMIT 1), 'Lenina 199',            '8-800-901-23-45')
                        ;";

                    GenerateGuids(command, 1, 9);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table SupplieItems
        public static async Task<bool> TryCreateTableSupplieItemsAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE supplie_items (
                            supplie_item_id UUID PRIMARY KEY,
                            order_date      DATE NOT NULL DEFAULT CURRENT_DATE,
                            supplier_id     UUID REFERENCES suppliers (supplier_id)
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO supplie_items (supplie_item_id, order_date, supplier_id)
                        VALUES
                        (@id1,  '2022-06-01', (SELECT supplier_id FROM suppliers WHERE name = 'FreshMeat'           LIMIT 1)),
                        (@id2,  '2022-06-01', (SELECT supplier_id FROM suppliers WHERE name = 'FreshVegetable'      LIMIT 1)),
                        (@id3,  '2022-06-01', (SELECT supplier_id FROM suppliers WHERE name = 'EduardStore'         LIMIT 1)),
                        (@id4,  '2022-06-01', (SELECT supplier_id FROM suppliers WHERE name = 'KhabarovskFish'      LIMIT 1)),
                        (@id5,  '2022-06-01', (SELECT supplier_id FROM suppliers WHERE name = 'Kolosok'             LIMIT 1)),
                        (@id6,  '2022-06-01', (SELECT supplier_id FROM suppliers WHERE name = 'SaintPetersburgFish' LIMIT 1)),
                        (@id7,  '2022-06-01', (SELECT supplier_id FROM suppliers WHERE name = 'ChickenFarm'         LIMIT 1)),
                        (@id8,  '2022-06-01', (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1)),
                        (@id9,  '2022-09-01', (SELECT supplier_id FROM suppliers WHERE name = 'FreshMeat'           LIMIT 1)),
                        (@id10, '2022-09-01', (SELECT supplier_id FROM suppliers WHERE name = 'FreshVegetable'      LIMIT 1)),
                        (@id11, '2022-09-01', (SELECT supplier_id FROM suppliers WHERE name = 'EduardStore'         LIMIT 1)),
                        (@id12, '2022-09-01', (SELECT supplier_id FROM suppliers WHERE name = 'KhabarovskFish'      LIMIT 1)),
                        (@id13, '2022-09-01', (SELECT supplier_id FROM suppliers WHERE name = 'Kolosok'             LIMIT 1)),
                        (@id14, '2022-09-01', (SELECT supplier_id FROM suppliers WHERE name = 'SaintPetersburgFish' LIMIT 1)),
                        (@id15, '2022-09-01', (SELECT supplier_id FROM suppliers WHERE name = 'ChickenFarm'         LIMIT 1)),
                        (@id16, '2022-09-01', (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1)),
                        (@id17, '2022-08-01', (SELECT supplier_id FROM suppliers WHERE name = 'AlcoMarket'          LIMIT 1))
                        ;";

                    GenerateGuids(command, 1, 17);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table Supplies
        public static async Task<bool> TryCreateTableSuppliesAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE supplies (
                            supplie_item_id UUID REFERENCES supplie_items (supplie_item_id) ON UPDATE CASCADE,
                            product_id      UUID REFERENCES products (product_id) ON UPDATE CASCADE,
                            amount          NUMERIC NOT NULL DEFAULT 1,
                            CONSTRAINT      supplie_pkey PRIMARY KEY (supplie_item_id, product_id)
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO supplies (supplie_item_id, product_id, amount)
                        VALUES
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'FreshMeat'           LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'meat_cutlet'        LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'FreshVegetable'      LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'tomato'             LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'FreshVegetable'      LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'cucumber'           LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'FreshVegetable'      LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'salad_pack'         LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'FreshVegetable'      LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'cheese_pack'        LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'EduardStore'         LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'cheese_pack'        LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'KhabarovskFish'      LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'fish_cutlet'        LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'Kolosok'             LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'bread'              LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SaintPetersburgFish' LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'fish_cutlet'        LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'ChickenFarm'         LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'chicken_cutlet'     LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'fish_sause_pack'    LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'chicken_sause_pack' LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'meat_sause_pack'    LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'bbq_sause_pack'     LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1) AND order_date = '2022-06-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'cheese_sause_pack'  LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'FreshMeat'           LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'meat_cutlet'        LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'FreshVegetable'      LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'tomato'             LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'FreshVegetable'      LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'cucumber'           LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'FreshVegetable'      LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'salad_pack'         LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'FreshVegetable'      LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'cheese_pack'        LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'EduardStore'         LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'cheese_pack'        LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'KhabarovskFish'      LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'fish_cutlet'        LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'Kolosok'             LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'bread'              LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SaintPetersburgFish' LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'fish_cutlet'        LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'ChickenFarm'         LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'chicken_cutlet'     LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'fish_sause_pack'    LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'chicken_sause_pack' LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'meat_sause_pack'    LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'bbq_sause_pack'     LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'SauseMaster'         LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'cheese_sause_pack'  LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'AlcoMarket'          LIMIT 1) AND order_date = '2022-08-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'Whiskey'            LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'AlcoMarket'          LIMIT 1) AND order_date = '2022-08-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'Vodka'              LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'AlcoMarket'          LIMIT 1) AND order_date = '2022-08-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'Martini'            LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'AlcoMarket'          LIMIT 1) AND order_date = '2022-08-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'Vine'               LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'AlcoMarket'          LIMIT 1) AND order_date = '2022-08-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'Absinthe'           LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'Kolosok'             LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'Cola'               LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'Kolosok'             LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'Fanta'              LIMIT 1), 10),
                        ((SELECT supplie_item_id FROM supplie_items WHERE supplier_id = (SELECT supplier_id FROM suppliers WHERE name = 'Kolosok'             LIMIT 1) AND order_date = '2022-09-01' LIMIT 1), (SELECT product_id FROM products WHERE name = 'Sprite'             LIMIT 1), 10)
                        ;";

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }
        
        // Create Table Orders
        public static async Task<bool> TryCreateTableOrdersAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE orders (
                            order_id    UUID PRIMARY KEY,
                            order_date  DATE NOT NULL DEFAULT CURRENT_DATE,
                            user_id     UUID REFERENCES users (user_id)
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO orders (order_id, order_date, user_id)
                        VALUES
                        (@id1, '2020-10-13', (SELECT user_id FROM users WHERE login = 'user'      LIMIT 1)),
                        (@id2, '2020-10-13', (SELECT user_id FROM users WHERE login = 'younguser' LIMIT 1)),
                        (@id3, '2020-10-14', (SELECT user_id FROM users WHERE login = 'user'      LIMIT 1)),
                        (@id4, '2020-10-14', (SELECT user_id FROM users WHERE login = 'younguser' LIMIT 1))
                        ;";

                    GenerateGuids(command, 1, 4);

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Create Table Purchases
        public static async Task<bool> TryCreateTablePurchacesAsync(string connectionString)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
            {
                await sqlConnection.OpenAsync();
                NpgsqlTransaction transaction = sqlConnection.BeginTransaction();

                NpgsqlCommand command = sqlConnection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = @"
                        CREATE TABLE purchases (
                            menu_item_id UUID REFERENCES menu_items (menu_item_id) ON UPDATE CASCADE,
                            order_id     UUID REFERENCES orders (order_id) ON UPDATE CASCADE ON DELETE CASCADE,
                            amount       NUMERIC NOT NULL DEFAULT 1,
                            CONSTRAINT   purchase_pkey PRIMARY KEY (menu_item_id, order_id)
                        );";

                    await command.ExecuteNonQueryAsync();

                    command.CommandText = @"
                        INSERT INTO purchases (menu_item_id, order_id, amount)
                        VALUES
                        ((SELECT menu_item_id FROM menu_items WHERE name = 'Extra Burger' LIMIT 1), (SELECT order_id FROM orders WHERE order_date = '2020-10-13' AND user_id = (SELECT user_id FROM users WHERE login = 'user'      LIMIT 1)), 1),
                        ((SELECT menu_item_id FROM menu_items WHERE name = 'BBQ Burger'   LIMIT 1), (SELECT order_id FROM orders WHERE order_date = '2020-10-13' AND user_id = (SELECT user_id FROM users WHERE login = 'user'      LIMIT 1)), 1),
                        ((SELECT menu_item_id FROM menu_items WHERE name = 'Whiskey'      LIMIT 1), (SELECT order_id FROM orders WHERE order_date = '2020-10-13' AND user_id = (SELECT user_id FROM users WHERE login = 'user'      LIMIT 1)), 1),
                        ((SELECT menu_item_id FROM menu_items WHERE name = 'Fishburger'   LIMIT 1), (SELECT order_id FROM orders WHERE order_date = '2020-10-13' AND user_id = (SELECT user_id FROM users WHERE login = 'younguser' LIMIT 1)), 1),
                        ((SELECT menu_item_id FROM menu_items WHERE name = 'Hamburger'    LIMIT 1), (SELECT order_id FROM orders WHERE order_date = '2020-10-14' AND user_id = (SELECT user_id FROM users WHERE login = 'user'      LIMIT 1)), 1),
                        ((SELECT menu_item_id FROM menu_items WHERE name = 'Hamburger'    LIMIT 1), (SELECT order_id FROM orders WHERE order_date = '2020-10-14' AND user_id = (SELECT user_id FROM users WHERE login = 'younguser' LIMIT 1)), 1)
                        ;";

                    await command.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return true;
        }

        // Private Methods
        private static string UpdateConnectionString(string connectionString, string dbName)
        {
            if (!connectionString.Contains($"Database=postgres"))
            {
                return connectionString;
            }

            return connectionString.Replace("Database=postgres", $"Database={dbName}");
        }

        private static void GenerateGuids(NpgsqlCommand command, int firstId, int lastId)
        {
            for (int currentId = firstId; currentId <= lastId; currentId++)
            {
                command.Parameters.Add(new NpgsqlParameter($"@id{currentId}", Guid.NewGuid()));
            }
        }
    }
}