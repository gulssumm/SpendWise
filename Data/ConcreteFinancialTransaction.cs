using System;

namespace Data
{
    public class ConcreteFinancialTransaction : FinancialTransaction
    {
        // Parameterless constructor required by Entity Framework
        public ConcreteFinancialTransaction() : base()
        {
        }

        public ConcreteFinancialTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
            : base(description, amount, isExpense, category, date)
        {
        }
    }
}