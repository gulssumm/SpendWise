using System;

namespace Data
{
    public class ConcreteUserEvent : UserEvent
    {
        public ConcreteUserEvent() : base() { }

        public ConcreteUserEvent(Guid userId, string description) : base(userId, description) { }
    }
}