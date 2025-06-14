using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsAsync()
        {
            return await _repository.GetTransactionsAsync();
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId)
        {
            return await _repository.GetTransactionsByUserAsync(userId);
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be null or empty", nameof(category));

            return await _repository.GetTransactionsByCategoryAsync(category);
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be later than end date");

            return await _repository.GetTransactionsByDateRangeAsync(startDate, endDate);
        }

        public async Task AddTransactionAsync(IFinancialTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            ValidateTransaction(transaction);
            await _repository.AddTransactionAsync(transaction);
        }

        public async Task UpdateTransactionAsync(IFinancialTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            ValidateTransaction(transaction);
            await _repository.UpdateTransactionAsync(transaction);
        }

        public async Task DeleteTransactionAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Transaction ID must be positive", nameof(id));

            await _repository.DeleteTransactionAsync(id);
        }

        public async Task<decimal> CalculateBalanceAsync()
        {
            var transactions = await GetTransactionsAsync();
            return transactions.Sum(t => t.IsExpense ? -t.Amount : t.Amount);
        }

        public async Task<decimal> CalculateBalanceByUserAsync(Guid userId)
        {
            var transactions = await GetTransactionsByUserAsync(userId);
            return transactions.Sum(t => t.IsExpense ? -t.Amount : t.Amount);
        }

        public async Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync()
        {
            var transactions = await GetTransactionsAsync();
            return transactions
                .Where(t => t.IsExpense)
                .GroupBy(t => t.Category)
                .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount));
        }

        public async Task<List<IFinancialTransaction>> GetRecentTransactionsAsync(int count = 10)
        {
            var transactions = await GetTransactionsAsync();
            return transactions
                .OrderByDescending(t => t.Date)
                .Take(count)
                .ToList();
        }

        // ✅ FIXED: Call async repository method directly
        public async Task<List<IUser>> GetUsersAsync()
        {
            return await _repository.GetUsersAsync();
        }

        public async Task<IUser> GetUserAsync(Guid id)
        {
            return await _repository.GetUserAsync(id);
        }

        public async Task AddUserAsync(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentException("User name cannot be null or empty");

            await _repository.AddUserAsync(user);
        }

        public async Task UpdateUserAsync(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentException("User name cannot be null or empty");

            await _repository.UpdateUserAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await _repository.DeleteUserAsync(id);
        }
        public async Task<List<ITransactionCategory>> GetCategoriesAsync()
        {
            return await _repository.GetCategoriesAsync();
        }

        public async Task AddCategoryAsync(ITransactionCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be null or empty");

            await _repository.AddCategoryAsync(category);
        }

        public async Task UpdateCategoryAsync(ITransactionCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be null or empty");

            await _repository.UpdateCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Category ID must be positive", nameof(id));

            await _repository.DeleteCategoryAsync(id);
        }

        public async Task<List<IEvent>> GetEventsAsync()
        {
            return await _repository.GetEventsAsync();
        }

        public async Task<List<IEvent>> GetEventsByUserAsync(Guid userId)
        {
            return await _repository.GetEventsByUserAsync(userId);
        }

        public async Task AddEventAsync(IEvent e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            await _repository.AddEventAsync(e);
        }

        private static void ValidateTransaction(IFinancialTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(transaction.Description))
                throw new ArgumentException("Transaction description cannot be null or empty");

            if (transaction.Amount <= 0)
                throw new ArgumentException("Transaction amount must be positive");

            if (string.IsNullOrWhiteSpace(transaction.Category))
                throw new ArgumentException("Transaction category cannot be null or empty");
        }
    }
}