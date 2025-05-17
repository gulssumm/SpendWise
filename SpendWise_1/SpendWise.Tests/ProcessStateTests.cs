using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendWise.Data;
using SpendWise.Data.Models;
using System;

namespace TestProject1
{
    [TestClass]
    public class ProcessStateTests
    {
        [TestMethod]
        public void AddTransaction_ShouldAffectProcessStateBalance()
        {
            // Arrange
            var state = new TransactionProcessState();

            var t1 = TestDataGenerator.GenerateTransactionModel("Groceries", 50.0m, true, "Food", DateTime.Now);
            var t2 = TestDataGenerator.GenerateTransactionModel("Freelance", 200.0m, false, "Work", DateTime.Now);

            // Act
            state.Transactions.Add(t1);
            state.Transactions.Add(t2);

            // Expected balance = income - expense = 200 - 50 = 150
            var expectedBalance = 150.0m;

            // Assert
            Assert.AreEqual(expectedBalance, state.CalculateBalance());
        }

        [TestMethod]
        public void AddTransaction_ShouldUpdateTransactionCount()
        {
            // Arrange
            var state = new TransactionProcessState();

            var t1 = TestDataGenerator.GenerateTransactionModel("T1", 100.0m, true, "Misc", DateTime.Now);
            var t2 = TestDataGenerator.GenerateTransactionModel("T2", 150.0m, false, "Income", DateTime.Now);

            // Act
            state.Transactions.Add(t1);
            state.Transactions.Add(t2);

            // Assert
            Assert.AreEqual(2, state.Transactions.Count);
        }

        [TestMethod]
        public void EmptyProcessState_ShouldHaveZeroBalance()
        {
            // Arrange
            var state = new TransactionProcessState();

            // Act
            var balance = state.CalculateBalance();

            // Assert
            Assert.AreEqual(0.0m, balance);
        }
    }
}