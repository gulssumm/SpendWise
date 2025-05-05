using System.Collections.Generic;

namespace SpendWise.Data
{
    public interface ITransactionRepository
    {
        void AddTransaction(FinancialTransaction transaction);
        List<FinancialTransaction> GetTransactions();

        void AddEvent(Event e);
        List<Event> GetEvents();

        void SaveTransactions(List<FinancialTransaction> transactions);
        List<FinancialTransaction> LoadTransactions();
    }

    
}
