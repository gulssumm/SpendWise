using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpendWise.Data.Models
{
    public class TransactionCategoryModel : CatalogItem
    {
        public TransactionCategoryModel(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}