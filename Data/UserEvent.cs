using System;

namespace Data
{
    public class UserEvent : Event
    {
        public UserEvent() : base() { }

        public UserEvent(Guid userId, string description)
        {
            UserId = userId;
            Description = description;
            Timestamp = DateTime.Now;
        }
    }
}
