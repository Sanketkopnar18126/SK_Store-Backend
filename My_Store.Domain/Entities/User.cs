using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public Guid PublicId { get; private set; }=Guid.NewGuid();
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "Customer";
        public string Phone { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public bool IsEmailConfirmed { get; private set; } = false;
        public List<RefreshToken> RefreshTokens { get; private set; } = new();

        protected User() { }

        public User(string fullName, string email, string passwordHash, string phone, string role = "Customer")
        {
            PublicId = Guid.NewGuid();
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            Phone = phone;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetPasswordHash(string hash)
        {
            PasswordHash = hash;
            UpdatedAt = DateTime.UtcNow;
        }
        public void ConfirmEmail()
        {
            IsEmailConfirmed = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void SetRole(string role)
        {
            Role = role;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
