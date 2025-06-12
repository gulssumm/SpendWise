using System.Configuration;
using Data;
using Logic;

namespace Presentation.Model
{
    /// <summary>
    /// Factory to create Model with all dependencies
    /// This encapsulates the dependency creation so View layer doesn't need to reference Data/Logic
    /// </summary>
    public static class DependencyFactory
    {
        public static FinancialDataModel CreateFinancialDataModel()
        {
            // Get connection string from App.config
            var connectionString = ConfigurationManager.ConnectionStrings["FinancialAppConnection"]?.ConnectionString
                ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FinancialApp;Integrated Security=True";

            // Create the dependency chain: Repository -> Service -> Model
            ITransactionRepository repository = new TransactionRepository(connectionString);
            ITransactionService transactionService = new TransactionService(repository);

            return new FinancialDataModel(transactionService);
        }
    }
}