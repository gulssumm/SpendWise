using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendWise.Data;
using System;
using System.Linq;

namespace TestProject1
{
    [TestClass]
    public class ProcessStateTests
    {
        [TestMethod]
        public void AddTransaction_ShouldAffectProcessStateBalance()
        {
            var state = new TransactionProcessState();

            var t1 = new FinancialTransaction("Groceries", 50.0m, true, "Food", DateTime.Now);
            var t2 = new FinancialTransaction("Freelance", 200.0m, false, "Work", DateTime.Now);

            state.Transactions.Add(t1);
            state.Transactions.Add(t2);

            var expectedBalance = 150.0m; // 200 income - 50 expense

            Assert.AreEqual(expectedBalance, state.CalculateBalance());
        }

        [TestMethod]
        public void AddTransaction_ShouldUpdateTransactionCount()
        {
            var state = new TransactionProcessState();

            state.Transactions.Add(TestDataGenerator.GenerateTransaction("T1", 100.0m, true, "Misc", DateTime.Now));
            state.Transactions.Add(TestDataGenerator.GenerateTransaction("T2", 150.0m, false, "Income", DateTime.Now));


            Assert.AreEqual(2, state.Transactions.Count);
        }

        [TestMethod]
        public void EmptyProcessState_ShouldHaveZeroBalance()
        {
            var state = new TransactionProcessState();

            var balance = state.CalculateBalance();

            Assert.AreEqual(0.0m, balance);
        }
    }
}
