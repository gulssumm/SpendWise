using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Data.Models
{
    public class Event
    {
        public Guid UserId { get; set; }
        public string Description { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}
