using System;
using System.Collections.Generic;

namespace SpendWise.Data.Models
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly List<FinancialTransactionModel> transactions = new List<FinancialTransactionModel>();
        private readonly List<EventModel> events = new List<EventModel>();

        public void AddTransaction(FinancialTransactionModel transaction)
        {
            transactions.Add(transaction);
        }

        public void SaveTransactions(List<FinancialTransactionModel> transactions)
        {
            this.transactions.Clear();
            this.transactions.AddRange(transactions);
        }

        public List<FinancialTransactionModel> LoadTransactions()
        {
            return transactions;
        }

        public List<FinancialTransactionModel> GetTransactions()
        {
            return transactions;
        }

        public void AddEvent(EventModel e)
        {
            events.Add(e);
        }

        public List<EventModel> GetEvents()
        {
            return events;
        }
    }
}
