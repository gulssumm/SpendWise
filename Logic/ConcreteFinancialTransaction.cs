using System;
using Data;

namespace Logic
{
    public class ConcreteFinancialTransaction : FinancialTransaction
    {
        public ConcreteFinancialTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
            : base(description, amount, isExpense, category, date)
        {
        }
    }
}
