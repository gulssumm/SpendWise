using System;
using System.Collections.Generic;

namespace Data
{
    public interface IDataFactory
    {
        IFinancialTransaction CreateTransaction(string description, decimal amount, bool isExpense, string category, DateTime date);
        IUser CreateUser(Guid id, string name);
        IEvent CreateEvent(Guid userId, string description);
        ITransactionCategory CreateCategory(string name, string description);

        // Data generation methods
        List<IFinancialTransaction> GenerateSampleTransactions(int count);
        List<IUser> GenerateSampleUsers(int count);
    }
}