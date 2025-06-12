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
    /// </summary>
    public class FinancialDataModel
    {
        private readonly ITransactionService _transactionService;

        public FinancialDataModel(ITransactionService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        // Transaction operations
        public async Task<List<IFinancialTransaction>> GetTransactionsAsync()
        {
            return await _transactionService.GetTransactionsAsync();
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId)
        {
            return await _transactionService.GetTransactionsByUserAsync(userId);
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByCategory(string category)
        {
            return await _transactionService.GetTransactionsByCategory(category);
        }

        public async Task<List<IFinancialTransaction>> GetRecentTransactionsAsync(int count = 10)
        {
            return await _transactionService.GetRecentTransactionsAsync(count);
        }

        // Creates FinancialTransaction here so ViewModel doesn't need Data reference
        public async Task<IFinancialTransaction> AddTransactionAsync(string description, decimal amount, bool isExpense, string category, DateTime date)
        {
            var transaction = new FinancialTransaction(description, amount, isExpense, category, date);
            await _transactionService.AddTransactionAsync(transaction);
            return transaction; // Return the created transaction for ViewModel to add to collection
        }

        public async Task UpdateTransactionAsync(IFinancialTransaction transaction)
        {
            await _transactionService.UpdateTransactionAsync(transaction);
        }

        public async Task DeleteTransactionAsync(int id)
        {
            await _transactionService.DeleteTransactionAsync(id);
        }

        // User operations
        public async Task<List<IUser>> GetUsersAsync()
        {
            return await _transactionService.GetUsersAsync();
        }

        public async Task<IUser> GetUserAsync(Guid id)
        {
            return await _transactionService.GetUserAsync(id);
        }

        public async Task AddUserAsync(IUser user)
        {
            await _transactionService.AddUserAsync(user);
        }

        public async Task UpdateUserAsync(IUser user)
        {
            await _transactionService.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await _transactionService.DeleteUserAsync(id);
        }

        // Category operations
        public async Task<List<ITransactionCategory>> GetCategoriesAsync()
        {
            return await _transactionService.GetCategoriesAsync();
        }

        public async Task AddCategoryAsync(ITransactionCategory category)
        {
            await _transactionService.AddCategoryAsync(category);
        }

        public async Task UpdateCategoryAsync(ITransactionCategory category)
        {
            await _transactionService.UpdateCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _transactionService.DeleteCategoryAsync(id);
        }

        // Business calculations
        public async Task<decimal> CalculateBalanceAsync()
        {
            return await _transactionService.CalculateBalanceAsync();
        }

        public async Task<decimal> CalculateBalanceByUserAsync(Guid userId)
        {
            return await _transactionService.CalculateBalanceByUserAsync(userId);
        }

        public async Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync()
        {
            return await _transactionService.GetExpensesByCategoryAsync();
        }

        // Event operations
        public async Task<List<IEvent>> GetEventsAsync()
        {
            return await _transactionService.GetEventsAsync();
        }

        public async Task<List<IEvent>> GetEventsByUserAsync(Guid userId)
        {
            return await _transactionService.GetEventsByUserAsync(userId);
        }

        public async Task AddEventAsync(IEvent e)
        {
            await _transactionService.AddEventAsync(e);
        }
    }
}