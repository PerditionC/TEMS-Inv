// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using TEMS.InventoryModel.util;

namespace Tems_Inventory.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class MediatorTests
    {
        /// <summary>
        /// any setup required
        /// </summary>
        [SetUp]
        public void setupTest()
        {
        }

        /// <summary>
        /// any cleanup required
        /// </summary>
        [TearDown]
        public void cleanupTest()
        {
        }

        private static readonly string REQUESTED_ACTION = "MyMediatorTestAction";
        private string actionReceived = null;

        /// <summary>
        /// example callback, sets actionReceived to param
        /// </summary>
        /// <param name="param"></param>
        private void DoAction(object param)
        {
            System.Diagnostics.Debug.Print("DoAction received param '" + param?.ToString() ?? "null" + "'");
            actionReceived = param as string;
        }

        [Test]
        public void TestRegisterAndUnregister()
        {
            Mediator.Register(REQUESTED_ACTION, DoAction);
            Mediator.Unregister(REQUESTED_ACTION, DoAction);
        }

        [Test]
        public void TestWithCallback()
        {
            Mediator.Register(REQUESTED_ACTION, DoAction);
            actionReceived = null;
            Mediator.InvokeCallback("Not_Valid", null);
            Assert.IsNull(actionReceived);
            Mediator.InvokeCallback(REQUESTED_ACTION, REQUESTED_ACTION);
            Assert.AreEqual(REQUESTED_ACTION, actionReceived);
            Mediator.InvokeCallback(REQUESTED_ACTION, null);
            Assert.IsNull(actionReceived);
            Mediator.Unregister(REQUESTED_ACTION, DoAction);
        }
    }
}