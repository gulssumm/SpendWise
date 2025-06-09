using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface ITransactionCategory : IEntity
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}
