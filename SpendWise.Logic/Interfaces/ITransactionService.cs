using System;
using System.Collections.Generic;
using System.Transactions;
using SpendWise.Logic.Models;

namespace SpendWise.Logic.Interfaces
{
    public interface ITransactionService
    {
        void AddTransaction(string description, decimal amount, bool isExpense, string category);
        List<FinancialTransaction> GetTransactions();
        decimal GetBalance();
        List<FinancialTransaction> GetMonthlyReport(int month, int year);
        void LoadTransactions();
        void SaveTransactions();
    }
}
