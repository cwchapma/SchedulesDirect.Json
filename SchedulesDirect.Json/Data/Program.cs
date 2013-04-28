using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulesDirect.Json.Data
{
    public class Program
    {
        public string programID;
        public string md5;
        public DateTime airDateTime;
        public int duration; // seconds
        public bool cc;
        public bool stereo;
        public string tvRating;
        public Info info;

        public override string ToString()
        {
            return "Program programID:" + programID;
        }
        public class Info
        {
            public string programID;
            public string md5;
            public string descriptionLanguage;
            public string colorCode;
            public string sourceType;
            public string showType;
            public string episodeTitle150;
            public string syndicatedEpisodeNumber;
            public string originalAirDate;
            public List<string> castAndCrew;
            public List<string> genres;
            public Titles titles;
            public Descriptions descriptions;

            public override string ToString()
            {
                return "Program.Info programID:" + programID + " - " + titles.title10 + " - " + episodeTitle150;
            }

            public class Titles
            {
                public string title120;
                public string title10;
            }

            public class Descriptions
            {
                public string description255;
                public string description100;
                public string description60;
                public string description40;
                public string alternateDescription100;
            }
        }
    }
}
