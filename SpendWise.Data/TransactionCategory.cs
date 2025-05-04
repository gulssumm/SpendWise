using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Data
{
    public class TransactionCategory: CatalogItem
    {
        public TransactionCategory(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
