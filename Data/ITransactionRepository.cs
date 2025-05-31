using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    public interface ITransactionRepository
    {
        // CRUD Operations
        Task AddTransactionAsync(FinancialTransaction transaction);
        Task<List<FinancialTransaction>> GetTransactionsAsync();
        Task<FinancialTransaction?> GetTransactionByIdAsync(int id);
        Task UpdateTransactionAsync(FinancialTransaction transaction);
        Task DeleteTransactionAsync(int id);

        Task AddEventAsync(Event e);
        Task<List<Event>> GetEventsAsync();

        Task SaveTransactionsAsync(List<FinancialTransaction> transactions);
        Task<List<FinancialTransaction>> LoadTransactionsAsync();

        // Synchronous versions for compatibility
        void AddTransaction(FinancialTransaction transaction);
        List<FinancialTransaction> GetTransactions();
        void AddEvent(Event e);
        List<Event> GetEvents();
        void SaveTransactions(List<FinancialTransaction> transactions);
        List<FinancialTransaction> LoadTransactions();
    }
}