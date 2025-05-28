using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Data
{
    public class AppDataContext : DataContext
    {
        public AppDataContext(string connection) : base(connection) { }

        public Table<User> Users => GetTable<User>();
        public Table<Event> Events => GetTable<Event>();
        public Table<FinancialTransaction> FinancialTransactions => GetTable<FinancialTransaction>();
        public Table<ProcessState> ProcessStates => GetTable<ProcessState>();
        // Add others as needed
    }
}
