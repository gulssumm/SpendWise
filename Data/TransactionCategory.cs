using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public abstract class TransactionCategory : CatalogItem
    {
        public TransactionCategory(string name, string description)
        {
            Name = name;
            Description = description;
        }

        // Parameterless constructor for Entity Framework
        protected TransactionCategory()
        {
            Name = "";
            Description = "";
        }
    }
}