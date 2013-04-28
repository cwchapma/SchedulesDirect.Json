using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulesDirect.Json.Data
{
    public class Lineup : Headend
    {
        public List<DeviceType> deviceTypes;
        public List<Station> stations;

        public override string ToString()
        {
            return name + ", " + location + "(devices:" + deviceTypes.Count() + ", stations:" + stations.Count() + ")";
        }
    }

    public class DeviceType
    {
        public string name;
        public List<ChannelMap> map;

        public override string ToString()
        {
            return name + " (Channels:" + map.Count() + ")";
        }
    }

    public class ChannelMap
    {
        public string channel;
        public int stationID;
        
        [NonSerialized]
        public Station station;
        [NonSerialized]
        public DeviceType deviceType;

        public override string ToString()
        {
            return channel + " " + station.callsign + " - " + station.name + " (" + deviceType.name + ")";
        }
    }

    public class Station
    {
        public string affiliate;
        public Broadcaster broadcaster;
        public string callsign;
        public string language;
        public string name;
        public int stationID;

        [NonSerialized]
        public List<ChannelMap> channelMaps = new List<ChannelMap>();

        public override string ToString()
        {
            return stationID.ToString().PadLeft(6) + " " + name + " (Channels:" + channelMaps.Count() + ")";
        }
    }

    public class Broadcaster
    {
        public string city;
        public string state;
        public string zipcode;
        public string country;

        public override string ToString()
        {
            return String.Join(", ", city, state, zipcode, country);
        }
    }
}
