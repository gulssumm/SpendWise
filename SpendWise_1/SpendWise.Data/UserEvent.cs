using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Data
{
    public class UserEvent : Event
    {
        public Guid UserId { get; set; }

        public UserEvent(Guid userId, string description)
        {
            UserId = userId;
            Description = description;
            Timestamp = DateTime.Now;
        }
    }
}