using System;
using System.Data.Linq.Mapping;

namespace Data
{
    [Table(Name = "Events")]
    public class Event : IEvent
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column]
        public Guid UserId { get; set; }

        [Column]
        public string Description { get; set; } = string.Empty;

        [Column]
        public DateTime Timestamp { get; set; }
    }
}