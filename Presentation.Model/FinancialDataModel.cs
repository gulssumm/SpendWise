using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logic;
using Data;

namespace Presentation.Model
{
    /// <summary>
    /// Model layer - Handles all interactions with Logic layer
    /// This is the only layer that can reference Logic and Data directly
    /// FIXED: Added virtual keyword to enable proper test overriding
    /// </summary>
    public class FinancialDataModel
    {
        private readonly ITransactionService _transactionService;

        public FinancialDataModel(ITransactionService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        // Transaction operations
        public virtual async Task<List<IFinancialTransaction>> GetTransactionsAsync()
        {
            return await _transactionService.GetTransactionsAsync();
        }

        public virtual async Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId)
        {
            return await _transactionService.GetTransactionsByUserAsync(userId);
        }

        public virtual async Task<List<IFinancialTransaction>> GetTransactionsByCategory(string category)
        {
            return await _transactionService.GetTransactionsByCategory(category);
        }

        public virtual async Task<List<IFinancialTransaction>> GetRecentTransactionsAsync(int count = 10)
        {
            return await _transactionService.GetRecentTransactionsAsync(count);
        }

        // Creates FinancialTransaction here so ViewModel doesn't need Data reference
        public virtual async Task<IFinancialTransaction> AddTransactionAsync(string description, decimal amount, bool isExpense, string category, DateTime date)
        {
            var transaction = new FinancialTransaction(description, amount, isExpense, category, date);
            await _transactionService.AddTransactionAsync(transaction);
            return transaction; // Return the created transaction for ViewModel to add to collection
        }

        public virtual async Task UpdateTransactionAsync(IFinancialTransaction transaction)
        {
            await _transactionService.UpdateTransactionAsync(transaction);
        }

        public virtual async Task DeleteTransactionAsync(int id)
        {
            await _transactionService.DeleteTransactionAsync(id);
        }

        // User operations
        public virtual async Task<List<IUser>> GetUsersAsync()
        {
            return await _transactionService.GetUsersAsync();
        }

        public virtual async Task<IUser> GetUserAsync(Guid id)
        {
            return await _transactionService.GetUserAsync(id);
        }

        public virtual async Task AddUserAsync(IUser user)
        {
            await _transactionService.AddUserAsync(user);
        }

        public virtual async Task UpdateUserAsync(IUser user)
        {
            await _transactionService.UpdateUserAsync(user);
        }

        public virtual async Task DeleteUserAsync(Guid id)
        {
            await _transactionService.DeleteUserAsync(id);
        }

        // Category operations
        public virtual async Task<List<ITransactionCategory>> GetCategoriesAsync()
        {
            return await _transactionService.GetCategoriesAsync();
        }

        public virtual async Task AddCategoryAsync(ITransactionCategory category)
        {
            await _transactionService.AddCategoryAsync(category);
        }

        public virtual async Task UpdateCategoryAsync(ITransactionCategory category)
        {
            await _transactionService.UpdateCategoryAsync(category);
        }

        public virtual async Task DeleteCategoryAsync(int id)
        {
            await _transactionService.DeleteCategoryAsync(id);
        }

        // Business calculations
        public virtual async Task<decimal> CalculateBalanceAsync()
        {
            return await _transactionService.CalculateBalanceAsync();
        }

        public virtual async Task<decimal> CalculateBalanceByUserAsync(Guid userId)
        {
            return await _transactionService.CalculateBalanceByUserAsync(userId);
        }

        public virtual async Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync()
        {
            return await _transactionService.GetExpensesByCategoryAsync();
        }

        // Event operations
        public virtual async Task<List<IEvent>> GetEventsAsync()
        {
            return await _transactionService.GetEventsAsync();
        }

        public virtual async Task<List<IEvent>> GetEventsByUserAsync(Guid userId)
        {
            return await _transactionService.GetEventsByUserAsync(userId);
        }

        public virtual async Task AddEventAsync(IEvent e)
        {
            await _transactionService.AddEventAsync(e);
        }
    }
}