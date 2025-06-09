using System.Collections.Generic;

namespace Data
{
    public abstract class TransactionProcessState : ProcessState
    {
        public new abstract IList<FinancialTransaction> Transactions { get; }

        public abstract decimal CalculateBalance();
    }
}