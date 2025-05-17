using System.Collections.Generic;

namespace SpendWise.Data.Models
{
    public interface ITransactionRepository
    {
        void AddTransaction(FinancialTransactionModel transaction);
        List<FinancialTransactionModel> GetTransactions();

        void AddEvent(EventModel e);
        List<EventModel> GetEvents();

        void SaveTransactions(List<FinancialTransactionModel> transactions);
        List<FinancialTransactionModel> LoadTransactions();
    }


}