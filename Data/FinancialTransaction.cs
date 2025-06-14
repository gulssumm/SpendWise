using System;
using System.Data.Linq.Mapping;

namespace Data
{
    [Table(Name = "FinancialTransactions")]
    public class FinancialTransaction : IFinancialTransaction
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Description { get; set; } = string.Empty;

        [Column(CanBeNull = false)]
        public decimal Amount { get; set; }

        [Column(CanBeNull = false)]
        public bool IsExpense { get; set; }

        [Column(CanBeNull = false)]
        public string Category { get; set; } = string.Empty;

        [Column(CanBeNull = false)]
        public DateTime Date { get; set; }

        [Column(CanBeNull = true)]
        public Guid? UserId { get; set; }

        // Constructors
        public FinancialTransaction() { }

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