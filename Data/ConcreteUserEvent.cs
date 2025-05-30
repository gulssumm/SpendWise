using System;

namespace Data
{
    public class ConcreteUserEvent : UserEvent
    {
        // Parameterless constructor required by Entity Framework
        public ConcreteUserEvent() : base(Guid.Empty, "")
        {
        }

        public ConcreteUserEvent(Guid userId, string description)
            : base(userId, description)
        {
        }
    }
}