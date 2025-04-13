using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendWise.Data.Models
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., "Food", "Rent", "Utilities"
        public string Description { get; set; }
    }
}

