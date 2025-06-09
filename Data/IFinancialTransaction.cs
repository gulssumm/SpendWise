using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IFinancialTransaction : IEntity
    {
        string Description { get; set; }
        decimal Amount { get; set; }
        bool IsExpense { get; set; }
        string Category { get; set; }
        DateTime Date { get; set; }
        Guid? UserId { get; set; }
    }
}
