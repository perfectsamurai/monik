using System.ComponentModel.DataAnnotations;

namespace monitoring.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите Логин")]
        [Display(Name = "Логин")]
        public string email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string password { get; set; }
    }
}
