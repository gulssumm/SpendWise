using Data;

namespace Logic
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        // This property only if necessary for testing
        public ITransactionRepository TransactionRepository => _transactionRepository;

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

            var transaction = new FinancialTransaction(description, amount, isExpense, category.Name, DateTime.Now);
            var transactionEvent = new UserEvent(user.Id,
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
            var state = new TransactionProcessState();

            foreach (var t in transactions)
            {
                if (t.IsExpense)
                    state.TotalExpenses += t.Amount;
                else
                    state.TotalIncome += t.Amount;
            }

            state.CurrentBalance = state.TotalIncome - state.TotalExpenses;
            state.Transactions = transactions;

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
    }
}
