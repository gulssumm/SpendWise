using System.Collections.Generic;

namespace SpendWise.Data
{
    // ✅ This is the missing interface
    public interface ITransactionRepository
    {
        void AddTransaction(FinancialTransaction transaction);
        List<FinancialTransaction> GetTransactions();

        void AddEvent(Event e);
        List<Event> GetEvents();

        // Optional if you're using them:
        void SaveTransactions(List<FinancialTransaction> transactions);
        List<FinancialTransaction> LoadTransactions();
    }

    
}
