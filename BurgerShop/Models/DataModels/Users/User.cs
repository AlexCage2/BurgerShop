using BurgerShop.Models.AuthenticationRequests;
using System.Globalization;

namespace BurgerShop.Models.DataModels.Users
{
    public class User
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public string Role { get; set; }


        // Mapping
        public static explicit operator User(RegistrationRequest registrationRequest)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Login = registrationRequest.Login.Trim().ToLower(),
                PasswordHash = null,
                PasswordSalt = null,
                FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(registrationRequest.FirstName.Trim().ToLower()),
                LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(registrationRequest.LastName.Trim().ToLower()),
                Age = registrationRequest.Age.Value,
                Role = "user"
            };
        }
    }
}
