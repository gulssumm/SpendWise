using System;
using System.Data.Linq.Mapping;

namespace Data
{
    public abstract class CatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}