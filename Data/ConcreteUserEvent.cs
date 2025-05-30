using System;

namespace Data
{
    public class ConcreteUserEvent : UserEvent
    {
        // Parameterless constructor required by Entity Framework
        public ConcreteUserEvent() : base()
        {
        }

        public ConcreteUserEvent(Guid userId, string description)
            : base(userId, description)
        {
            // UserId and other properties are set by base constructor
        }
    }
}