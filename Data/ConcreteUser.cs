using System;

namespace Data
{
    public class ConcreteUser : User
    {
        public ConcreteUser()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
        }

        public ConcreteUser(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}