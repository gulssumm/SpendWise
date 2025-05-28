using System;
using Data;

namespace Logic
{
    public class ConcreteUser : User
    {
        public ConcreteUser()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
        }

        public ConcreteUser(Guid Id, string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
