using System.Collections.Generic;

namespace Data
{
    public abstract class ProcessState
    {
        public decimal CurrentBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }

        public List<FinancialTransaction> Transactions { get; set; } = new List<FinancialTransaction>();
    }
}
