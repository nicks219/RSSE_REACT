using System.ComponentModel.DataAnnotations;

namespace RandomSongSearchEngine.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; }

        public LoginModel(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
