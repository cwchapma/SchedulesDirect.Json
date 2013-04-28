using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulesDirect.Json
{
    public class ResponseException: Exception
    {
        public ResponseException(Response response)
            : base(response.ToString())
        {
            Response = response;
        }

        public ResponseException(string message, Response response) : base(message)
        {
            Response = response;
        }

        public Response Response { get; set; }
    }
}
