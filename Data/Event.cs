using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public abstract class Event
    {
        public int Id { get; set; } // Primary key for database
        public Guid UserId { get; set; }
        public string Description { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}