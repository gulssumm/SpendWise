using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Data
{
    public class TransactionProcessState : ProcessState
    {
        public TransactionProcessState()
        {
            Transactions = new List<FinancialTransaction>();
        }

        public decimal CalculateBalance()
        {
            return Transactions.Sum(t => t.IsExpense ? -t.Amount : t.Amount);
        }

    }
}