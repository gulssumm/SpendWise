using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Data.Models
{
    public class TransactionProcessState : ProcessState
    {
        public TransactionProcessState()
        {
            Transactions = new List<FinancialTransactionModel>();
        }

        public decimal CalculateBalance()
        {
            return Transactions.Sum(t => t.IsExpense ? -t.Amount : t.Amount);
        }

    }
}