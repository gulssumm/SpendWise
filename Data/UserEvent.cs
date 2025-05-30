using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UserEvent : Event
    {
        public UserEvent(Guid userId, string description)
        {
            UserId = userId;  // This sets the base class property
            Description = description;
            Timestamp = DateTime.Now;
        }

        // Parameterless constructor for Entity Framework
        public UserEvent() : base()
        {
        }
    }
}