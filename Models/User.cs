using System.ComponentModel.DataAnnotations;

namespace TodoList.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public byte[] PasswordSalt { get; set; } = new byte[0];
        public byte[] PasswordHash { get; set; } = new byte[0];
    }
}
