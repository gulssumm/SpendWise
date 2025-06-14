using System;
using System.Data.Linq.Mapping;

namespace Data
{
    [Table(Name = "Events")]
    public class Event : IEvent
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public Guid UserId { get; set; }

        [Column(CanBeNull = false)]
        public string Description { get; set; } = string.Empty;

        [Column(CanBeNull = false)]
        public DateTime Timestamp { get; set; }

        // Constructor
        public Event()
        {
            Timestamp = DateTime.Now;
        }
    }
}