
using System.ComponentModel.DataAnnotations;

namespace BusinessAccessLayer.DTOS.AuthDtos
{
    public class LoginDto
    {
        [Required, EmailAddress, StringLength(128)]
        public string Email { get; set; }

        [Required, StringLength(256)]
        public string Password { get; set; }
    }
}
