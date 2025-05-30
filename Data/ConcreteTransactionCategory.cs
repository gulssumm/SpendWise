using System;

namespace Data
{
    public class ConcreteTransactionCategory : TransactionCategory
    {
        // Parameterless constructor for Entity Framework
        public ConcreteTransactionCategory() : base("", "")
        {
        }

        public ConcreteTransactionCategory(string name, string description)
            : base(name, description)
        {
        }
    }
}