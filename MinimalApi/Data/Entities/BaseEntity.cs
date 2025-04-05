using System.Security.Cryptography;

namespace MinimalApi.Data.Entities
{
    public class BaseEntity<TID>
    {
        public TID Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool lastActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
