using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic
{
    public interface ITransactionService
    {
        // CRUD Operations 
        Task AddTransactionAsync(string description, decimal amount, bool isExpense, TransactionCategory category, User user);
        Task<List<FinancialTransaction>> GetTransactionsAsync();
        Task UpdateTransactionAsync(int id, string description, decimal amount, bool isExpense, string category);
        Task DeleteTransactionAsync(int id);

        // Additional operations
        Task<decimal> GetBalanceAsync();
        Task<ProcessState?> GetProcessStateAsync();
        Task SaveTransactionsAsync();
        Task LoadTransactionsAsync();
        Task<List<FinancialTransaction>> GetMonthlyReportAsync(int month, int year);

        // Synchronous versions for compatibility
        void AddTransaction(string description, decimal amount, bool isExpense, TransactionCategory category, User user);
        List<FinancialTransaction> GetTransactions();
        decimal GetBalance();
        ProcessState? GetProcessState();
        void SaveTransactions();
        void LoadTransactions();
        List<FinancialTransaction> GetMonthlyReport(int month, int year);
    }
}