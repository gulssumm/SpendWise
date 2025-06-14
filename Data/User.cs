using System;
using System.Data.Linq.Mapping;

namespace Data
{
    [Table(Name = "Users")]
    public class User : IUser
    {
        [Column(IsPrimaryKey = true)]
        public Guid Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        // Constructor
        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}