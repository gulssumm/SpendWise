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
            return await Task.Run(() => _repository.GetTransactions());
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByUserAsync(Guid userId)
        {
            return await Task.Run(() => _repository.GetTransactionsByUser(userId));
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be null or empty", nameof(category));

            return await Task.Run(() => _repository.GetTransactionsByCategory(category));
        }

        public async Task<List<IFinancialTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date cannot be later than end date");

            return await Task.Run(() => _repository.GetTransactionsByDateRange(startDate, endDate));
        }

        public async Task AddTransactionAsync(IFinancialTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            ValidateTransaction(transaction);
            await Task.Run(() => _repository.AddTransaction(transaction));
        }

        public async Task UpdateTransactionAsync(IFinancialTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            ValidateTransaction(transaction);
            await Task.Run(() => _repository.UpdateTransaction(transaction));
        }

        public async Task DeleteTransactionAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Transaction ID must be positive", nameof(id));

            await Task.Run(() => _repository.DeleteTransaction(id));
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

        public async Task<List<IUser>> GetUsersAsync()
        {
            return await Task.Run(() => _repository.GetUsers());
        }

        public async Task<IUser> GetUserAsync(Guid id)
        {
            return await Task.Run(() => _repository.GetUser(id));
        }

        public async Task AddUserAsync(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentException("User name cannot be null or empty");

            await Task.Run(() => _repository.AddUser(user));
        }

        public async Task UpdateUserAsync(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Name))
                throw new ArgumentException("User name cannot be null or empty");

            await Task.Run(() => _repository.UpdateUser(user));
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await Task.Run(() => _repository.DeleteUser(id));
        }

        public async Task<List<ITransactionCategory>> GetCategoriesAsync()
        {
            return await Task.Run(() => _repository.GetCategories());
        }

        public async Task AddCategoryAsync(ITransactionCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be null or empty");

            await Task.Run(() => _repository.AddCategory(category));
        }

        public async Task UpdateCategoryAsync(ITransactionCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be null or empty");

            await Task.Run(() => _repository.UpdateCategory(category));
        }

        public async Task DeleteCategoryAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Category ID must be positive", nameof(id));

            await Task.Run(() => _repository.DeleteCategory(id));
        }

        public async Task<List<IEvent>> GetEventsAsync()
        {
            return await Task.Run(() => _repository.GetEvents());
        }

        public async Task<List<IEvent>> GetEventsByUserAsync(Guid userId)
        {
            return await Task.Run(() => _repository.GetEventsByUser(userId));
        }

        public async Task AddEventAsync(IEvent e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            await Task.Run(() => _repository.AddEvent(e));
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