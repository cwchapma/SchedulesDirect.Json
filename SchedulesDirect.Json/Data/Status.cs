using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulesDirect.Json.Data
{
    public class Status
    {
        public Account account;
        public List<StatusHeadend> headend;
        public DateTime lastDataUpdate;
        public DateTime lastMetaDataUpdate;
        public List<Notification> notifications;
        public List<SystemStatus> systemStatus;
        public string response;
        public string serverID;
    }

    public class Account
    {
        public DateTime expires;
        public List<Message> messages;
        public int maxHeadends;
        public DateTime nextSuggestedConnectTime;
    }

    public class Message
    {
        public string msgID;
        public DateTime date;
        public string message;
    }

    public class StatusHeadend
    {
        public string ID;
        public DateTime modified; 
    }

    public class Notification
    {
        public string msgID;
        public DateTime date;
        public string message;
    }

    public class SystemStatus
    {
        public DateTime date;
        public string status;
        public string details;
    }
}
