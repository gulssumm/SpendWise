using SpendWise;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendWise.Logic.Models;

namespace SpendWise.Tests
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void Transaction_ShouldStoreCorrectValues()
        {
            // Arrange
            var transaction = new FinancialTransaction("Groceries", 50.0m, true, "Food", DateTime.Now);

            // Act & Assert
            Assert.AreEqual("Groceries", transaction.Description);
            Assert.AreEqual(50.0m, transaction.Amount);
            Assert.IsTrue(transaction.IsExpense);
            Assert.AreEqual("Food", transaction.Category);
        }

        [TestMethod]
        public void Adding_Transaction_Increases_List_Count()
        {
            // Arrange
            var transactions = new List<FinancialTransaction>();

            // Act
            transactions.Add(new FinancialTransaction("Salary", 1000.0m, false, "Income", DateTime.Now));

            // Assert
            Assert.AreEqual(1, transactions.Count);
        }

        [TestMethod]
        public void CheckBalance_Should_Calculate_Correctly()
        {
            // Arrange
            var transactions = new List<FinancialTransaction>
            {
                new FinancialTransaction("Salary", 2000.0m, false, "Income", DateTime.Now),
                new FinancialTransaction("Rent", 800.0m, true, "Bills", DateTime.Now),
                new FinancialTransaction("Shopping", 200.0m, true, "Other", DateTime.Now)
            };

            // Act
            decimal balance = transactions.Sum(t => t.IsExpense ? -t.Amount : t.Amount);

            // Assert
            Assert.AreEqual(1000.0m, balance);
        }
    }
}
