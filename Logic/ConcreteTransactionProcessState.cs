using System;
using System.Collections.Generic;
using System.Linq;
using Data;

namespace Logic
{
    public class ConcreteTransactionProcessState : TransactionProcessState
    {
        private readonly List<FinancialTransaction> _transactions;

        public ConcreteTransactionProcessState()
        {
            _transactions = new List<FinancialTransaction>();
        }

        public override IList<FinancialTransaction> Transactions => _transactions;

        public void AddTransaction(FinancialTransaction transaction)
        {
            _transactions.Add(transaction);
        }

        public override decimal CalculateBalance()
        {
            return _transactions.Sum(t => t.IsExpense ? -t.Amount : t.Amount);
        }
    }
}
