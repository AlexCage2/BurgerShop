using System.ComponentModel.DataAnnotations;

namespace BurgerShop.Models.AuthenticationRequests
{
    public class RegistrationRequest
    {
        [Required(ErrorMessage = "Логин должен быть указан")]
        [Display(Name = "Логин")]
        [StringLength(30, ErrorMessage = "Логин должен быть не более 30 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль должен быть указан")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "Пароль должен быть не более 30 символов")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Подтверждение пароля должно быть указано")]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Имя должно быть указано")]
        [Display(Name = "Имя")]
        [StringLength(30, ErrorMessage = "Имя должно быть не более 30 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия должна быть указана")]
        [Display(Name = "Фамилия")]
        [StringLength(30, ErrorMessage = "Фамилия должна быть не более 30 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Возраст должен быть указан")]
        [Display(Name = "Возраст")]
        [Range(1, 150, ErrorMessage = "Недопустимый возраст")]
        public int? Age { get; set; }
    }
}
