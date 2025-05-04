using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Data
{
    public class TransactionProcessState: ProcessState
    {
        public TransactionProcessState()
        {
            Transactions = new List<FinancialTransaction>();
        }
    }
}
