using System.Data.Linq.Mapping;

namespace Data
{
    [Table(Name = "TransactionCategories")]
    public class TransactionCategory : CatalogItem, ITransactionCategory
    {
        public TransactionCategory() : base() { }

        public TransactionCategory(string name, string description)
        {
            Name = name;
            Description = description;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public new int Id { get; set; }

        [Column]
        public new string Name { get; set; } = string.Empty;

        [Column]
        public new string Description { get; set; } = string.Empty;
    }
}