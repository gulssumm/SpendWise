using System;

namespace Data
{
    public class ConcreteTransactionCategory : TransactionCategory
    {
        public ConcreteTransactionCategory() : base() { }

        public ConcreteTransactionCategory(string name, string description)
            : base(name, description) { }
    }
}