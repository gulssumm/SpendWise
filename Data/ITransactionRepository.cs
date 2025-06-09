using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    public interface ITransactionRepository
    {
        // Transaction operations - using interfaces
        List<IFinancialTransaction> GetTransactions();
        List<IFinancialTransaction> GetTransactionsByUser(Guid userId);
        List<IFinancialTransaction> GetTransactionsByCategory(string category);
        List<IFinancialTransaction> GetTransactionsByDateRange(DateTime startDate, DateTime endDate);
        void AddTransaction(IFinancialTransaction transaction);
        void UpdateTransaction(IFinancialTransaction transaction);
        void DeleteTransaction(int id);

        // Async operations
        Task<List<IFinancialTransaction>> GetTransactionsAsync();
        Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId);
        Task<List<IFinancialTransaction>> GetTransactionsByCategoryAsync(string category);
        Task<List<IFinancialTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task AddTransactionAsync(IFinancialTransaction transaction);
        Task UpdateTransactionAsync(IFinancialTransaction transaction);
        Task DeleteTransactionAsync(int id);

        // User operations - using interfaces
        List<IUser> GetUsers();
        IUser GetUser(Guid id);
        void AddUser(IUser user);
        void UpdateUser(IUser user);
        void DeleteUser(Guid id);
        Task<List<IUser>> GetUsersAsync();
        Task<IUser> GetUserAsync(Guid id);
        Task AddUserAsync(IUser user);
        Task UpdateUserAsync(IUser user);
        Task DeleteUserAsync(Guid id);

        // Event operations - using interfaces
        List<IEvent> GetEvents();
        List<IEvent> GetEventsByUser(Guid userId);
        void AddEvent(IEvent e);
        Task<List<IEvent>> GetEventsAsync();
        Task<List<IEvent>> GetEventsByUserAsync(Guid userId);
        Task AddEventAsync(IEvent e);

        // Category operations - using interfaces
        List<ITransactionCategory> GetCategories();
        void AddCategory(ITransactionCategory category);
        void UpdateCategory(ITransactionCategory category);
        void DeleteCategory(int id);
        Task<List<ITransactionCategory>> GetCategoriesAsync();
        Task AddCategoryAsync(ITransactionCategory category);
        Task UpdateCategoryAsync(ITransactionCategory category);
        Task DeleteCategoryAsync(int id);
    }
}