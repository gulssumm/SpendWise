using System;
using System.Collections.Generic;
using SpendWise.Data.Interfaces;
using SpendWise.Data.Models;

namespace SpendWise.Data.Repositeries
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly List<FinancialTransaction> transactions = new();
        private readonly List<Event> events = new();

        public void AddTransaction(FinancialTransaction transaction)
        {
            transactions.Add(transaction);
        }

        public void SaveTransactions(List<FinancialTransaction> transactions)
        {
            this.transactions.Clear();
            this.transactions.AddRange(transactions);
        }

        public List<FinancialTransaction> LoadTransactions()
        {
            return transactions;
        }


        public List<FinancialTransaction> GetTransactions()
        {
            return transactions;
        }

        public void AddEvent(Event e)
        {
            events.Add(e);
        }

        public List<Event> GetEvents()
        {
            return events;
        }
    }
}
