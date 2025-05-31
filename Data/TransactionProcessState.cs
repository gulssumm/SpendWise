using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public abstract class TransactionProcessState : ProcessState
    {
        public new abstract IList<FinancialTransaction> Transactions { get; } // to explicitly hide base property

        public abstract decimal CalculateBalance();
    }
}