using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulesDirect.Json.Data
{
    public class HeadendResponse : Response
    {
        public List<Headend> data;
    }

    public class Headend
    {
        public string headend;
        public string name;
        public string location;

        public override string ToString()
        {
            return headend + ": " + name + " (" + location + ")";
        }
    }
}
