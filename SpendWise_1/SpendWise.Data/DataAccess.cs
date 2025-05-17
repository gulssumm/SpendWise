using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpendWise.Data;

namespace SpendWise.Data
{
    public class DataAccess
    {
        public void RunQueries()
        {
            using (var db = new MyDataModelDataContext())
            {
                // Method Syntax: Get all users
                var users = db.Users.ToList();

                // Query Syntax: Get all events for a specific user
                var userId = Guid.Parse("PUT-USER-ID-HERE");
                var userEvents = from e in db.Events
                                 where e.UserId == userId
                                 select e;

                // Method Syntax: Get all expense transactions sorted by amount descending
                var expenses = db.FinancialTransactions
                                 .Where(t => t.IsExpense)
                                 .OrderByDescending(t => t.Amount)
                                 .ToList();

                // Query Syntax: Get category names with their total transaction count
                var categoryStats = from t in db.FinancialTransactions
                                    group t by t.CategoryId into g
                                    select new
                                    {
                                        CategoryId = g.Key,
                                        Count = g.Count()
                                    };

                foreach (var stat in categoryStats)
                {
                    Console.WriteLine($"Category {stat.CategoryId} has {stat.Count} transactions.");
                }
            }
        }
    }

}
