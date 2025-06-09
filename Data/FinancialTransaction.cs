using System;
using System.Data.Linq.Mapping;

namespace Data
{
    [Table(Name = "Transactions")]
    public class FinancialTransaction : IFinancialTransaction
    {
        public FinancialTransaction() { }

        public FinancialTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
        {
            Description = description;
            Amount = amount;
            IsExpense = isExpense;
            Category = category;
            Date = date;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column]
        public string Description { get; set; } = string.Empty;

        [Column]
        public decimal Amount { get; set; }

        [Column]
        public bool IsExpense { get; set; }

        [Column]
        public string Category { get; set; } = string.Empty;

        [Column]
        public DateTime Date { get; set; }

        [Column]
        public Guid? UserId { get; set; }
    }
}