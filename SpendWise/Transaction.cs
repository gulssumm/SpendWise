using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise
{
    public class Transaction
    {
        public string Description { get; }
        public decimal Amount { get; }
        public bool IsExpense { get; }
        public string Category { get; }
        public DateTime Date { get; }

        public Transaction(string description, decimal amount, bool isExpense, string category, DateTime date)
        {
            Description = description;
            Amount = amount;
            IsExpense = isExpense;
            Category = category;
            Date = date;
        }
    }
}

