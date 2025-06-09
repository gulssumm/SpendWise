using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public class ConcreteTransactionProcessState : TransactionProcessState
    {
        private readonly List<FinancialTransaction> _transactions = new List<FinancialTransaction>();

        public override IList<FinancialTransaction> Transactions => _transactions;

        public void AddTransaction(FinancialTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            _transactions.Add(transaction);
            UpdateTotals();
        }

        public void RemoveTransaction(FinancialTransaction transaction)
        {
            if (_transactions.Remove(transaction))
            {
                UpdateTotals();
            }
        }

        public override decimal CalculateBalance()
        {
            decimal balance = 0;
            foreach (var t in _transactions)
            {
                balance += t.IsExpense ? -t.Amount : t.Amount;
            }
            return balance;
        }

        private void UpdateTotals()
        {
            TotalIncome = _transactions.Where(t => !t.IsExpense).Sum(t => t.Amount);
            TotalExpenses = _transactions.Where(t => t.IsExpense).Sum(t => t.Amount);
            CurrentBalance = CalculateBalance();
        }
    }
}