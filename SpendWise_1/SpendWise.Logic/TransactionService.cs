using System;
using System.Collections.Generic;
using System.Linq;
using SpendWise.Data;
using SpendWise.Data.Models;
using SpendWise.Logic.Interfaces;

namespace SpendWise.Logic.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public void AddTransaction(string description, decimal amount, bool isExpense, TransactionCategoryModel category, UserModel user)
        {
            var transaction = new FinancialTransactionModel(description, amount, isExpense, category.Name, DateTime.Now);
            var transactionEvent = new UserEventModel(user.Id,
                $"User {user.Name} added transaction: {description}, Amount: {amount}, Category: {category.Name}");

            _transactionRepository.AddTransaction(transaction);
            _transactionRepository.AddEvent(transactionEvent);
        }

        public List<FinancialTransactionModel> GetTransactions()
        {
            return _transactionRepository.GetTransactions();
        }

        public ProcessState GetProcessState()
        {
            var transactions = _transactionRepository.GetTransactions();
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
            var transactions = _transactionRepository.GetTransactions();
            _transactionRepository.SaveTransactions(transactions);
        }

        public void LoadTransactions()
        {
            var loadedTransactions = _transactionRepository.LoadTransactions();
            foreach (var transaction in loadedTransactions)
            {
                _transactionRepository.AddTransaction(transaction);
            }
        }

        public List<FinancialTransactionModel> GetMonthlyReport(int month, int year)
        {
            return _transactionRepository
                .GetTransactions()
                .Where(t => t.Date.Month == month && t.Date.Year == year)
                .ToList();
        }

        public decimal GetBalance()
        {
            var transactions = _transactionRepository.GetTransactions();
            decimal income = transactions.Where(t => !t.IsExpense).Sum(t => t.Amount);
            decimal expense = transactions.Where(t => t.IsExpense).Sum(t => t.Amount);
            return income - expense;
        }
    }
}