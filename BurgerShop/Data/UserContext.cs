using BurgerShop.Models.DataModels.Users;
using Npgsql;

namespace BurgerShop.Data
{
    public sealed class UserContext
    {
        private readonly string _connectionString;

        public UserContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> TryCreateUserAsync(User user)
        {
            try
            {
                using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    string sqlQuery = @"
                        INSERT INTO users 
                        VALUES (
                            @id,
                            @login,
                            @passHash,
                            @passSalt,
                            @firstName,
                            @lastName,
                            @age,
                            (SELECT role_id FROM user_roles WHERE role = @role LIMIT 1));";

                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, sqlConnection);

                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@login", user.Login);
                    command.Parameters.AddWithValue("@passHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@passSalt", user.PasswordSalt);
                    command.Parameters.AddWithValue("@firstName", user.FirstName);
                    command.Parameters.AddWithValue("@lastName", user.LastName); 
                    command.Parameters.AddWithValue("@age", user.Age);
                    command.Parameters.AddWithValue("@role", user.Role);

                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<User> GetUserAsync(string login, CancellationToken cancelationToken = default)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                        SELECT 
                            u.user_id,
                            u.login,
                            u.passwordHash,
                            u.passwordSalt,
                            u.firstName,
                            u.lastName,
                            u.age,
                            ur.role
                        FROM users AS u
                            JOIN user_roles AS ur
                            ON u.role_id = ur.role_id
                        WHERE u.login = @login
                        LIMIT 1;";

                NpgsqlCommand command = new NpgsqlCommand(sqlQuery, sqlConnection);

                command.Parameters.AddWithValue("@login", login.Trim().ToLower());

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancelationToken))
                {
                    if (reader.HasRows && !reader.IsClosed)
                    {
                        while (await reader.ReadAsync())
                        {                            
                            User user = new User
                            {
                                Id = reader.GetGuid(0),
                                Login = reader.GetString(1),
                                PasswordHash = (byte[])reader[2],
                                PasswordSalt = (byte[])reader[3],
                                FirstName = reader.GetString(4),
                                LastName = reader.GetString(5),
                                Age = reader.GetInt32(6),
                                Role = reader.GetString(7)
                            };

                            return user;
                        }
                    }                    
                }
            }

            return null;
        }

        public async Task<bool> TryUpdateUserAsync(User user)
        {
            try
            {
                using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
                {
                    await sqlConnection.OpenAsync();

                    string sqlQuery = @"
                        UPDATE users 
                        SET (
                            @id,
                            @login,
                            @passHash,
                            @passSalt,
                            @firstName,
                            @lastName,
                            @age,
                            @role)
                        WHERE id = @id;";

                    NpgsqlCommand command = new NpgsqlCommand(sqlQuery, sqlConnection);

                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@login", user.Login);
                    command.Parameters.AddWithValue("@passHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@passSalt", user.PasswordSalt);
                    command.Parameters.AddWithValue("@firstName", user.FirstName);
                    command.Parameters.AddWithValue("@lastName", user.LastName);
                    command.Parameters.AddWithValue("@age", user.Age);
                    command.Parameters.AddWithValue("@role", user.Role);

                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsUserExistsAsync(string Login, CancellationToken cancellationToken = default)
        {
            using (NpgsqlConnection sqlConnection = new NpgsqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                string sqlQuery = @"
                    SELECT count(*)
                    FROM users
                    WHERE login = @login;
                    ";

                NpgsqlCommand command = new NpgsqlCommand(sqlQuery, sqlConnection);

                command.Parameters.AddWithValue("@login", Login);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    if (reader.HasRows && !reader.IsClosed)
                    {
                        while(await reader.ReadAsync())
                        {
                            if (reader.GetInt32(0) > 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            };

            return false;
        }
    }
}
