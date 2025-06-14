using System.Data.Linq.Mapping;

namespace Data
{
    [Table(Name = "TransactionCategories")]
    public class TransactionCategory : ITransactionCategory
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; } = string.Empty;

        [Column(CanBeNull = true)]
        public string Description { get; set; } = string.Empty;

        // Constructors
        public TransactionCategory() { }

        public TransactionCategory(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
