using System;
using System.Collections.Generic;
using System.Linq;
using SpendWise.Data;
using SpendWise.Logic.Interfaces;

namespace SpendWise.Logic
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        public void AddTransaction(string description, decimal amount, bool isExpense, CatalogItem category, User user)
        {
            var transaction = new FinancialTransaction(description, amount, isExpense, category.Name, DateTime.Now);
            var transactionEvent = new UserEvent(user.Id,
                $"User {user.Name} added transaction: {description}, Amount: {amount}, Category: {category.Name}");

            transactionRepository.AddTransaction(transaction);
            transactionRepository.AddEvent(transactionEvent);
        }

        public List<FinancialTransaction> GetTransactions()
        {
            return transactionRepository.GetTransactions();
        }

        public ProcessState GetProcessState()
        {
            var transactions = transactionRepository.GetTransactions();
            var state = new TransactionProcessState();

            foreach (var t in transactions)
            {
                if (t.IsExpense)
                    state.TotalExpenses += t.Amount;
                else
                    state.TotalIncome += t.Amount;
            }

            state.CurrentBalance = state.TotalIncome - state.TotalExpenses;
            state.Transactions = transactions;

            return state;
        }

        public void SaveTransactions()
        {
            var transactions = transactionRepository.GetTransactions();
            transactionRepository.SaveTransactions(transactions);
        }

        public void LoadTransactions()
        {
            var loadedTransactions = transactionRepository.LoadTransactions();
            foreach (var transaction in loadedTransactions)
            {
                transactionRepository.AddTransaction(transaction);
            }
        }

        public List<FinancialTransaction> GetMonthlyReport(int month, int year)
        {
            return transactionRepository
                .GetTransactions()
                .Where(t => t.Date.Month == month && t.Date.Year == year)
                .ToList();
        }

        public decimal GetBalance()
        {
            var transactions = GetTransactions();
            decimal income = transactions.Where(t => !t.IsExpense).Sum(t => t.Amount);
            decimal expense = transactions.Where(t => t.IsExpense).Sum(t => t.Amount);
            return income - expense;
        }

    }
}
