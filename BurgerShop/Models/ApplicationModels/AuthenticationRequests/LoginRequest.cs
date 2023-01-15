using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.ApplicationModels.AuthenticationRequests
{
    public class LoginRequest
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Логин должен быть указан")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Пароль должен быть указан")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
