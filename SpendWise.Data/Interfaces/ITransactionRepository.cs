using System.Collections.Generic;
using SpendWise.Data.Models;

namespace SpendWise.Data.Interfaces
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
