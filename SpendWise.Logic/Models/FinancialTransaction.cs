using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpendWise.Logic.Models;

namespace SpendWise.Logic.Models
{
    public class FinancialTransaction
    {
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
    }
}

