using System.ComponentModel.DataAnnotations;
using System.Globalization;
namespace TaskTracker.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Username { get; set; } = "";

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = "";

    }
}
