using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        //public string? CreatedByIp { get; private set; }

        public Guid UserPublicId { get; private set; }
        public User? User { get; private set; }

        protected RefreshToken() { }

        public RefreshToken(string token, DateTime expiresAt, string? createdByIp, Guid userId)
        {
            Token = token;
            ExpiresAt = expiresAt;
            UserPublicId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        public void Revoke() => IsRevoked = true;
        public bool IsActive() => !IsRevoked && DateTime.UtcNow < ExpiresAt;
    }
}
