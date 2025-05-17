using System;
using System.Collections.Generic;
using SpendWise.Logic.Interfaces;
using SpendWise.Data.Models;

namespace SpendWise.Presentation.Models
{
    /// <summary>
    /// Presentation model that wraps the logic layer transaction service.
    /// Acts as a facade to simplify interaction between ViewModels and the business logic.
    /// </summary>
    public class TransactionModel
    {
        private readonly ITransactionService _transactionService;

        public TransactionModel(ITransactionService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        /// <summary>
        /// Gets all transactions from the system
        /// </summary>
        public List<FinancialTransactionModel> GetAllTransactions()
        {
            return _transactionService.GetTransactions();
        }

        /// <summary>
        /// Gets the current balance (income - expenses)
        /// </summary>
        public decimal GetBalance()
        {
            return _transactionService.GetBalance();
        }

        /// <summary>
        /// Adds a new transaction to the system
        /// </summary>
        public void AddTransaction(string description, decimal amount, bool isExpense,
                                  TransactionCategoryModel category, UserModel user)
        {
            _transactionService.AddTransaction(description, amount, isExpense, category, user);
        }

        /// <summary>
        /// Gets transactions for a specific month and year
        /// </summary>
        public List<FinancialTransactionModel> GetMonthlyReport(int month, int year)
        {
            return _transactionService.GetMonthlyReport(month, year);
        }

        /// <summary>
        /// Gets the current process state including transactions and balance
        /// </summary>
        public ProcessState GetProcessState()
        {
            return _transactionService.GetProcessState();
        }

        /// <summary>
        /// Saves all transactions to persistent storage
        /// </summary>
        public void SaveTransactions()
        {
            _transactionService.SaveTransactions();
        }

        /// <summary>
        /// Loads transactions from persistent storage
        /// </summary>
        public void LoadTransactions()
        {
            _transactionService.LoadTransactions();
        }
    }
}