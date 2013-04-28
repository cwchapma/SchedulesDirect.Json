using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.Linq;
using SchedulesDirect.Json;
using SchedulesDirect.Json.Data;
using System.Configuration;

namespace UnitTestSchedulesDirect
{
    [TestClass]
    public class TestConnect
    {
        private SchedulesDirectApi sd;

        [TestInitialize]
        public void Initialize()
        {
            sd = SdFactory.CreateAuthenticated();

            var headends = new List<Headend>();
            try
            {
                headends = sd.GetHeadends();
            }
            catch { }

            if (!headends.Exists(he => he.headend == "PC:K2J1T2"))
            {
                sd.AddHeadend("PC:K2J1T2");
            }
            if (!headends.Exists(he => he.headend == "0005410"))
            {
                sd.AddHeadend("0005410");
            }
        }

        [TestMethod]
        public void TestStatus()
        {
            var status = sd.Status;
            Debug.WriteLine("Expires: " + sd.Status.account.expires.ToShortDateString());
        }

        [TestMethod]
        public void TestLineups()
        {
            var headends = sd.GetHeadends().Select(he => he.headend);
            var lineups = sd.GetLineups(headends);

            foreach (var headend in headends)
            {
                Assert.IsTrue(lineups.Exists(lineup => lineup.headend == headend), "Asked for a lineup for headend:\"" + headend + "\" and didn't get it");
            }
        }

        [TestMethod]
        public void TestSchedules()
        {
            var stations = new List<int>() { 10036, 10050 };
            var schedules = sd.GetSchedules(stations).ToList();

            foreach (var station in stations)
            {
                Assert.IsTrue(schedules.Exists(schedule => schedule.stationId == station), "Asked for schedule for station:" + station + " and didn't get it");
            }
        }

        [TestMethod]
        public void TestPrograms()
        {
            var schedules = sd.GetSchedules(new List<int>() { 10036 });
            var programs = sd.FillInProgramInfo(schedules.First().programs).ToList();
        }

        [TestMethod]
        public void TestMetaData()
        {
            var metadata = sd.GetMetadata().ToList();
        }
    }
}
