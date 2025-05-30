using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        // Parameterless constructor for when DI is not set up
        public TransactionService()
        {
            _transactionRepository = new TransactionRepository();
        }

        public void AddTransaction(string description, decimal amount, bool isExpense, TransactionCategory category, User user)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.", nameof(description));

            if (category == null)
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");

            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");

            // Create instances using Logic layer implementations
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

        public ProcessState GetProcessState()
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

        // Logic layer implementations - separate from Data layer concrete classes
        private class LogicFinancialTransaction : FinancialTransaction
        {
            // Parameterless constructor required
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
            // Parameterless constructor required
            public LogicUserEvent() : base(Guid.Empty, "")
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