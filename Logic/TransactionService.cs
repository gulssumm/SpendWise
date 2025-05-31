using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public TransactionService()
        {
            _transactionRepository = new TransactionRepository();
        }

        // Async CRUD Operations
        public async Task AddTransactionAsync(string description, decimal amount, bool isExpense, TransactionCategory category, User user)
        {
            ValidateTransaction(description, amount, category, user);

            var transaction = new LogicFinancialTransaction(description, amount, isExpense, category.Name, DateTime.Now);
            var transactionEvent = new LogicUserEvent(user.Id,
                $"User {user.Name} added transaction: {description}, Amount: {amount}, Category: {category.Name}");

            await _transactionRepository.AddTransactionAsync(transaction);
            await _transactionRepository.AddEventAsync(transactionEvent);
        }

        public async Task<List<FinancialTransaction>> GetTransactionsAsync()
        {
            return await _transactionRepository.GetTransactionsAsync();
        }

        public async Task UpdateTransactionAsync(int id, string description, decimal amount, bool isExpense, string category)
        {
            var existingTransaction = await _transactionRepository.GetTransactionByIdAsync(id);
            if (existingTransaction != null)
            {
                var updatedTransaction = new LogicFinancialTransaction(description, amount, isExpense, category, existingTransaction.Date)
                {
                    Id = id
                };
                await _transactionRepository.UpdateTransactionAsync(updatedTransaction);
            }
        }

        public async Task DeleteTransactionAsync(int id)
        {
            await _transactionRepository.DeleteTransactionAsync(id);
        }

        public async Task<decimal> GetBalanceAsync()
        {
            var transactions = await _transactionRepository.GetTransactionsAsync();
            decimal income = transactions.Where(t => !t.IsExpense).Sum(t => t.Amount);
            decimal expense = transactions.Where(t => t.IsExpense).Sum(t => t.Amount);
            return income - expense;
        }

        public async Task<ProcessState?> GetProcessStateAsync()
        {
            var transactions = await _transactionRepository.GetTransactionsAsync();
            var state = new LogicTransactionProcessState();

            foreach (var t in transactions)
            {
                if (t.IsExpense)
                    state.TotalExpenses += t.Amount;
                else
                    state.TotalIncome += t.Amount;

                state.Transactions.Add(t);
            }

            state.CurrentBalance = state.TotalIncome - state.TotalExpenses;
            return state;
        }

        public async Task SaveTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetTransactionsAsync();
            await _transactionRepository.SaveTransactionsAsync(transactions);
        }

        public async Task LoadTransactionsAsync()
        {
            var loadedTransactions = await _transactionRepository.LoadTransactionsAsync();
            foreach (var transaction in loadedTransactions)
            {
                await _transactionRepository.AddTransactionAsync(transaction);
            }
        }

        public async Task<List<FinancialTransaction>> GetMonthlyReportAsync(int month, int year)
        {
            var transactions = await _transactionRepository.GetTransactionsAsync();
            return transactions
                .Where(t => t.Date.Month == month && t.Date.Year == year)
                .ToList();
        }

        // Synchronous versions for compatibility
        public void AddTransaction(string description, decimal amount, bool isExpense, TransactionCategory category, User user)
        {
            ValidateTransaction(description, amount, category, user);

            var transaction = new LogicFinancialTransaction(description, amount, isExpense, category.Name, DateTime.Now);
            var transactionEvent = new LogicUserEvent(user.Id,
                $"User {user.Name} added transaction: {description}, Amount: {amount}, Category: {category.Name}");

            _transactionRepository.AddTransaction(transaction);
            _transactionRepository.AddEvent(transactionEvent);
        }

        public List<FinancialTransaction> GetTransactions()
        {
            return _transactionRepository.GetTransactions();
        }

        public ProcessState? GetProcessState()
        {
            var transactions = _transactionRepository.GetTransactions();
            var state = new LogicTransactionProcessState();

            foreach (var t in transactions)
            {
                if (t.IsExpense)
                    state.TotalExpenses += t.Amount;
                else
                    state.TotalIncome += t.Amount;

                state.Transactions.Add(t);
            }

            state.CurrentBalance = state.TotalIncome - state.TotalExpenses;
            return state;
        }

        public void SaveTransactions()
        {
            var transactions = _transactionRepository.GetTransactions();
            _transactionRepository.SaveTransactions(transactions);
        }

        public void LoadTransactions()
        {
            var loadedTransactions = _transactionRepository.LoadTransactions();
            foreach (var transaction in loadedTransactions)
            {
                _transactionRepository.AddTransaction(transaction);
            }
        }

        public List<FinancialTransaction> GetMonthlyReport(int month, int year)
        {
            return _transactionRepository
                .GetTransactions()
                .Where(t => t.Date.Month == month && t.Date.Year == year)
                .ToList();
        }

        public decimal GetBalance()
        {
            var transactions = _transactionRepository.GetTransactions();
            decimal income = transactions.Where(t => !t.IsExpense).Sum(t => t.Amount);
            decimal expense = transactions.Where(t => t.IsExpense).Sum(t => t.Amount);
            return income - expense;
        }

        private static void ValidateTransaction(string description, decimal amount, TransactionCategory? category, User? user)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.", nameof(description));

            if (category == null)
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");

            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        // Logic layer implementations
        private class LogicFinancialTransaction : FinancialTransaction
        {
            public LogicFinancialTransaction() : base()
            {
            }

            public LogicFinancialTransaction(string description, decimal amount, bool isExpense, string category, DateTime date)
                : base(description, amount, isExpense, category, date)
            {
            }
        }

        private class LogicUserEvent : UserEvent
        {
            public LogicUserEvent() : base()
            {
            }

            public LogicUserEvent(Guid userId, string description)
                : base(userId, description)
            {
            }
        }

        private class LogicTransactionProcessState : TransactionProcessState
        {
            private readonly List<FinancialTransaction> _transactions = new();

            public override IList<FinancialTransaction> Transactions => _transactions;

            public override decimal CalculateBalance()
            {
                return _transactions.Sum(t => t.IsExpense ? -t.Amount : t.Amount);
            }
        }
    }
}