using SpendWise.Data;
using SpendWise.Data.Models;
using System.Collections.Generic;

namespace SpendWise.Logic.Interfaces
{
    public interface ITransactionService
    {
        void AddTransaction(string description, decimal amount, bool isExpense, TransactionCategoryModel category, UserModel user);
        List<FinancialTransactionModel> GetTransactions();
        decimal GetBalance();
        ProcessState GetProcessState();
        void SaveTransactions();
        void LoadTransactions();
        List<FinancialTransactionModel> GetMonthlyReport(int month, int year);
    }
}