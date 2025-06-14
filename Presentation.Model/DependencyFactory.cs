using System;
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
        // Default connection string for development
        private const string DefaultConnectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FinancialApp;Integrated Security=True";

        /// <summary>
        /// Creates FinancialDataModel with configuration from App.config
        /// </summary>
        public static FinancialDataModel CreateFinancialDataModel()
        {
            var connectionString = GetConnectionString();
            return CreateFinancialDataModel(connectionString);
        }

        /// <summary>
        /// Creates FinancialDataModel with custom connection string
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        public static FinancialDataModel CreateFinancialDataModel(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = DefaultConnectionString;
            }

            // Create the dependency chain: Repository -> Service -> Model
            ITransactionRepository repository = new TransactionRepository(connectionString);
            ITransactionService transactionService = new TransactionService(repository);

            return new FinancialDataModel(transactionService);
        }

        /// <summary>
        /// Creates FinancialDataModel for testing with mock service
        /// </summary>
        /// <param name="mockService">Mock transaction service for testing</param>
        public static FinancialDataModel CreateFinancialDataModelForTesting(ITransactionService mockService)
        {
            if (mockService == null)
            {
                throw new ArgumentNullException(nameof(mockService));
            }
            return new FinancialDataModel(mockService);
        }

        /// <summary>
        /// Creates FinancialDataModel with test database connection
        /// </summary>
        public static FinancialDataModel CreateFinancialDataModelForTesting()
        {
            const string testConnectionString =
                "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FinancialAppTest;Integrated Security=True";

            return CreateFinancialDataModel(testConnectionString);
        }

        /// <summary>
        /// Gets connection string from App.config with fallback
        /// </summary>
        private static string GetConnectionString()
        {
            try
            {
                // Try to get connection string from App.config
                var connectionStringSettings = ConfigurationManager.ConnectionStrings["FinancialAppConnection"];
                if (connectionStringSettings != null && !string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
                {
                    return connectionStringSettings.ConnectionString;
                }
            }
            catch (Exception)
            {
                // If ConfigurationManager fails, use default
            }

            // Fallback to default connection string
            return DefaultConnectionString;
        }
    }
}