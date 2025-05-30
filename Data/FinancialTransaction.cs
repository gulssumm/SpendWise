using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public abstract class FinancialTransaction
    {
        public int Id { get; set; } // Primary key for database
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public bool IsExpense { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }

        public FinancialTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
        {
            Description = description;
            Amount = amount;
            IsExpense = isExpense;
            Category = category;
            Date = date;
        }

        // Parameterless constructor for Entity Framework
        protected FinancialTransaction()
        {
            Description = "";
            Category = "";
            Date = DateTime.Now;
        }
    }
}