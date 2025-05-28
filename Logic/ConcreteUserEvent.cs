using System;
using Data;

namespace Logic
{
    public class ConcreteUserEvent : UserEvent
    {
        public ConcreteUserEvent(Guid userId, string description)
            : base(userId, description)
        {
        }
    }
}
