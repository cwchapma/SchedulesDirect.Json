using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulesDirect.Json
{
    public class Response
    {
        public string response;
        public int code;
        public string serverID;
        public string message;

        public override string ToString()
        {
            return response + " (" + code.ToString() + "): " + message;
        }
    }

    public class ZipResponse : Response
    {
        public string filename;
        public string URL;
        public DateTime datetime;
    }

    public class Randhash : Response
    {
        public string randhash;
    }

    public class ModifyHeadendsResponse : Response
    {
        public int changesRemaining;
    }
}
