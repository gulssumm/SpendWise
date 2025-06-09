using System.Data.Linq;

namespace Data
{
    public class FinancialDataContext : DataContext
    {
        public Table<FinancialTransaction> Transactions { get { return GetTable<FinancialTransaction>(); } }
        public Table<Event> Events { get { return GetTable<Event>(); } }
        public Table<User> Users { get { return GetTable<User>(); } }
        public Table<TransactionCategory> TransactionCategories { get { return GetTable<TransactionCategory>(); } }

        public FinancialDataContext(string connectionString) : base(connectionString) { }
    }
}