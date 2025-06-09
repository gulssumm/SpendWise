using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic
{
    public interface ITransactionService
    {
        // Transaction methods
        Task<List<IFinancialTransaction>> GetTransactionsAsync();
        Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId);
        Task<List<IFinancialTransaction>> GetTransactionsByCategory(string category);
        Task<List<IFinancialTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task AddTransactionAsync(IFinancialTransaction transaction);
        Task UpdateTransactionAsync(IFinancialTransaction transaction);
        Task DeleteTransactionAsync(int id);

        // Balance calculation methods
        Task<decimal> CalculateBalanceAsync();
        Task<decimal> CalculateBalanceByUserAsync(Guid userId);
        Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync();
        Task<List<IFinancialTransaction>> GetRecentTransactionsAsync(int count = 10);

        // User methods
        Task<List<IUser>> GetUsersAsync();
        Task<IUser> GetUserAsync(Guid id);
        Task AddUserAsync(IUser user);
        Task UpdateUserAsync(IUser user);
        Task DeleteUserAsync(Guid id);

        // Category methods
        Task<List<ITransactionCategory>> GetCategoriesAsync();
        Task AddCategoryAsync(ITransactionCategory category);
        Task UpdateCategoryAsync(ITransactionCategory category);
        Task DeleteCategoryAsync(int id);

        // Event methods
        Task<List<IEvent>> GetEventsAsync();
        Task<List<IEvent>> GetEventsByUserAsync(Guid userId);
        Task AddEventAsync(IEvent e);
    }
}