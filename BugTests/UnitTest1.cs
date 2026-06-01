using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using System;

namespace BugTests
{
    [TestClass]
    public class BugWorkflowTests
    {
        [TestMethod]
        public void Test1_StartState()
        {
            Assert.AreEqual(State.Analysis, new Bug().CurrentState);
        }

        [TestMethod]
        public void Test2_New_Analyze()
        {
            var b = new Bug();
            Assert.AreEqual(State.Analysis, b.CurrentState);
        }

        [TestMethod]
        public void Test3_Analysis_Reject()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.Reject);
            Assert.AreEqual(State.Rejected, b.CurrentState);
        }

        [TestMethod]
        public void Test4_Analysis_StartFix()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            Assert.AreEqual(State.Resolution, b.CurrentState);
        }

        [TestMethod]
        public void Test5_Analysis_AskInfo()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.AskInfo);
            Assert.AreEqual(State.NeedMoreInfo, b.CurrentState);
        }

        [TestMethod]
        public void Test6_NeedMoreInfo_To_Analysis()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.AskInfo);
            b.machine.Fire(Trigger.ToResolution);
            Assert.AreEqual(State.Analysis, b.CurrentState);
        }

        [TestMethod]
        public void Test7_NeedMoreInfo_To_Resolution()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.AskInfo);
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.ToResolution);
            Assert.AreEqual(State.Resolution, b.CurrentState);
        }

        [TestMethod]
        public void Test8_Resolution_Success()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.VerifySuccess);
            Assert.AreEqual(State.Closed, b.CurrentState);
        }

        [TestMethod]
        public void Test9_Resolution_Fail()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.VerifyFailure);
            Assert.AreEqual(State.Analysis, b.CurrentState);
        }

        [TestMethod]
        public void Test10_Resolution_CantRepro()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.CannotReproduce);
            Assert.AreEqual(State.NotReproducible, b.CurrentState);
        }

        [TestMethod]
        public void Test11_Resolution_To_NeedMoreInfo()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.AskInfo);
            Assert.AreEqual(State.NeedMoreInfo, b.CurrentState);
        }

        [TestMethod]
        public void Test12_Review_Close()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.CannotReproduce);
            b.machine.Fire(Trigger.ConfirmNotRepro);
            Assert.AreEqual(State.Closed, b.CurrentState);
        }

        [TestMethod]
        public void Test13_Review_Return()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.CannotReproduce);
            b.machine.Fire(Trigger.ConfirmBugExists);
            Assert.AreEqual(State.Analysis, b.CurrentState);
        }

        [TestMethod]
        public void Test14_Closed_Reopen()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.VerifySuccess);
            b.machine.Fire(Trigger.Reopen);
            Assert.AreEqual(State.Analysis, b.CurrentState);
        }

        [TestMethod]
        public void Test15_Reopened_Analyze()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.VerifySuccess);
            b.machine.Fire(Trigger.Reopen);
            Assert.AreEqual(State.Analysis, b.CurrentState);
        }

        [TestMethod]
        public void Test16_Invalid_New_Reject()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.Reject);
            Assert.ThrowsException<Exception>(() => b.machine.Fire(Trigger.Reject));
        }

        [TestMethod]
        public void Test17_Invalid_Analysis_Success()
        {
            var b = new Bug();
            Assert.ThrowsException<Exception>(() => b.machine.Fire(Trigger.VerifySuccess));
        }

        [TestMethod]
        public void Test18_Invalid_Resolution_DirectClose()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            Assert.ThrowsException<Exception>(() => b.machine.Fire(Trigger.ConfirmNotRepro));
        }

        [TestMethod]
        public void Test19_Invalid_Analysis_CantRepro()
        {
            var b = new Bug();
            Assert.ThrowsException<Exception>(() => b.machine.Fire(Trigger.CannotReproduce));
        }

        [TestMethod]
        public void Test20_Invalid_Returned_Reopen()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.Reject);
            Assert.ThrowsException<Exception>(() => b.machine.Fire(Trigger.Reopen));
        }

        [TestMethod]
        public void Test21_Invalid_New_StartFix()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            Assert.ThrowsException<Exception>(() => b.machine.Fire(Trigger.VerifySuccess));
        }

        [TestMethod]
        public void Test22_Invalid_Closed_VerifySuccess()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.VerifySuccess);
            Assert.ThrowsException<Exception>(() => b.machine.Fire(Trigger.VerifySuccess));
        }

        [TestMethod]
        public void Test23_Invalid_Review_StartFix()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.ToResolution);
            b.machine.Fire(Trigger.CannotReproduce);
            Assert.ThrowsException<Exception>(() => b.machine.Fire(Trigger.ToResolution));
        }

        [TestMethod]
        public void Test24_Invalid_NeedMoreInfo_Reject()
        {
            var b = new Bug();
            b.machine.Fire(Trigger.AskInfo);
            Assert.ThrowsException<Exception>(() => b.machine.Fire(Trigger.Reject));
        }
    }
}