using SpendWise.Data.Models;
using System.Collections.Generic;

namespace SpendWise.Logic.Interfaces
{
    public interface ITransactionService
    {
        void AddTransaction(string description, decimal amount, bool isExpense, CatalogItem category, User user);
        List<FinancialTransaction> GetTransactions();
        decimal GetBalance();
        ProcessState GetProcessState();
        void SaveTransactions();
        void LoadTransactions();
        List<FinancialTransaction> GetMonthlyReport(int month, int year);
    }
}
