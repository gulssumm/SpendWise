using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public abstract class CatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = ""; // Initialize to avoid nullable warning
        public string Description { get; set; } = ""; 
    }
}