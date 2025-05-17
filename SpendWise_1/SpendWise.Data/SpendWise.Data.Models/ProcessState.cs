using System;
using System.Collections.Generic;

namespace SpendWise.Data.Models
{
    public abstract class ProcessState
    {
        public decimal CurrentBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }

        public List<FinancialTransactionModel> Transactions { get; set; } = new List<FinancialTransactionModel>();
    }
}
