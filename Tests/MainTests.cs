using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remote;

namespace Tests
{
    [TestClass]
    public class MainTests
    {
        [TestMethod]
        public void TestGetProjects()
        {
            SharedType st = new SharedType();
            var t = st.getProjects();
            if (t.Length<=0)
            {
                Assert.Fail("Проектов нет");
            }
        }
        [TestMethod]
        public void UpdateProject()
        {
            Server srv = new Server();
            Client ct = new Client();
            ct.GetUpdates("Test");
        }
    }
}
