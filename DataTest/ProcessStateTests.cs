using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data;
using System;

namespace DataTest
{
    [TestClass]
    public class ProcessStateTests
    {
        [TestMethod]
        public void AddTransaction_ShouldAffectProcessStateBalance()
        {
            var state = new ConcreteTransactionProcessState();

            var t1 = new FinancialTransaction("Groceries", 50.0m, true, "Food", DateTime.Now);
            var t2 = new FinancialTransaction("Freelance", 200.0m, false, "Work", DateTime.Now);

            state.AddTransaction(t1);
            state.AddTransaction(t2);

            var expectedBalance = 150.0m; // 200 income - 50 expense

            Assert.AreEqual(expectedBalance, state.CalculateBalance());
        }

        [TestMethod]
        public void AddTransaction_ShouldUpdateTransactionCount()
        {
            var state = new ConcreteTransactionProcessState();

            state.AddTransaction(TestDataGenerator.GenerateTransaction("T1", 100.0m, true, "Misc", DateTime.Now));
            state.AddTransaction(TestDataGenerator.GenerateTransaction("T2", 150.0m, false, "Income", DateTime.Now));

            Assert.AreEqual(2, state.Transactions.Count);
        }

        [TestMethod]
        public void EmptyProcessState_ShouldHaveZeroBalance()
        {
            var state = new ConcreteTransactionProcessState();

            var balance = state.CalculateBalance();

            Assert.AreEqual(0.0m, balance);
        }

        [TestMethod]
        public void AddTransaction_ShouldUpdateTransactionCount_UsingInlineCreation()
        {
            var state = new ConcreteTransactionProcessState();

            var t1 = new FinancialTransaction("Inline T1", 100.0m, true, "Misc", DateTime.Now);
            var t2 = new FinancialTransaction("Inline T2", 150.0m, false, "Income", DateTime.Now);

            state.AddTransaction(t1);
            state.AddTransaction(t2);

            Assert.AreEqual(2, state.Transactions.Count);
        }
    }
}
