using SchedulesDirect.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestSchedulesDirect
{
    class SdFactory
    {
        public static SchedulesDirectApi CreateAuthenticated()
        {
            //sd = new SchedulesDirectApi();
            var sd = new SchedulesDirectApi("http://23.21.174.111", 20130311, "handleRequest.php");

            var username = ConfigurationManager.AppSettings["username"];
            var password = ConfigurationManager.AppSettings["password"];

            sd.Authenticate(username, password);

            return sd;
        }
    }
}
