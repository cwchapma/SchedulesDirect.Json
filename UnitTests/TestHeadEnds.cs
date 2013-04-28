using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchedulesDirect.Json;

namespace UnitTestSchedulesDirect
{
    [TestClass]
    public class TestHeadEnds
    {
        private SchedulesDirectApi sd;
        const string postalCode = "K2J1T2";

        [TestInitialize]
        public void Initialize()
        {
            sd = SdFactory.CreateAuthenticated();
        }

        [TestMethod]
        public void TestListHeadendsByPC()
        {
            var headends = sd.GetHeadends(postalCode);
            foreach (var headend in headends)
            {
                Console.WriteLine(headend);
            }
        }

        [TestMethod]
        public void TestAddHeadEnd()
        {
            var headend = "PC:" + postalCode;

            var headends = sd.GetHeadends();
            if (headends.Exists(he => he.headend == headend))
            {
                sd.DeleteHeadend(headend);
            }
            //foreach (var he in headends)
            //{
            //    sd.DeleteHeadend(he.headend);
            //}
            //headends = sd.GetHeadends();

            var response = sd.AddHeadend(headend);

            //var headends = sd.GetHeadends();
        }

        [TestMethod, ExpectedException(typeof(ResponseException))]
        public void TestDoubleDelete()
        {
            sd.DeleteHeadend("PC:" + postalCode);
            sd.DeleteHeadend("PC:" + postalCode);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }
    }
}
