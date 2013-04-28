using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using SchedulesDirect.Json.Data;
using SchedulesDirect.Json.Helpers;

namespace SchedulesDirect.Json
{
    //public class JsonObject
    //{
    //    public override string ToString()
    //    {
    //        return Json.Serialize(this);
    //    }
    //}

    public class SchedulesDirectApi
    {
        public string Url { get; set; }
        public int ApiVersion { get; set; }
        public string Page { get; set; }

        public SchedulesDirectApi(string baseUrl = "https://data2.schedulesdirect.org", int apiVersion = 20130224, string page = "request.php")
        {
            Url = baseUrl;
            ApiVersion = apiVersion;
            Page = page;

            IsAuthenticated = false;

            client = new HttpClient();
            client.BaseAddress = new Uri(Url);

            Debug.WriteLine("URL: " + Url);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private HttpClient client;

        private string randhash;

        public bool Authenticate(string username, string password)
        {
            return AuthenticateSHA1(username, eSHA1Encoder.Encode(password));
        }

        public bool AuthenticateSHA1(string username, string sha1Password)
        {
            var request = new Command()
            {
                request = new Authentication() { username = username, password = sha1Password },
                @object = "randhash",
                action = "get",
                api = ApiVersion,
            };

            var result = Post<Randhash>(request);

            var success = result.response == "OK";
            if (success)
            {
                randhash = result.randhash;
                IsAuthenticated = true;
            }
            return success;
        }

        public bool IsAuthenticated { get; private set; }

        public Status Status
        {
            get
            {
                CheckAuthentication();

                var request = new AuthenticatedCommand()
                {
                    randhash = randhash,
                    @object = "status",
                    action = "get",
                    api = ApiVersion,
                };

                return Post<Status>(request);
            }
        }

        private void CheckAuthentication()
        {
            if (!IsAuthenticated) { throw new UnauthorizedAccessException("You must be authenticated before using this method"); }
        }

        public List<Headend> GetHeadends()
        {
            CheckAuthentication();
            var command = new AuthenticatedCommand()
            {
                randhash = randhash,
                @object = "headends",
                action = "get",
                api = ApiVersion,
            };
            var response = Post<HeadendResponse>(command);

            return response.data;
        }

        public List<Headend> GetHeadends(string postalCode)
        {
            CheckAuthentication();
            var command = new AuthenticatedCommand()
            {
                randhash = randhash,
                request = "PC:" + postalCode,
                @object = "headends",
                action = "get",
                api = ApiVersion,
            };
            var response = Post<HeadendResponse>(command);

            return response.data;
        }

        public ModifyHeadendsResponse AddHeadend(string headend)
        {
            CheckAuthentication();

            var command = new AuthenticatedCommand()
            {
                randhash = randhash,
                @object = "headends",
                action = "add",
                request = headend,
                api = ApiVersion,
            };
            return Post<ModifyHeadendsResponse>(command);
        }

        public ModifyHeadendsResponse DeleteHeadend(string headend)
        {
            CheckAuthentication();

            var command = new AuthenticatedCommand()
            {
                randhash = randhash,
                @object = "headends",
                action = "delete",
                request = headend,
                api = ApiVersion,
            };
            return Post<ModifyHeadendsResponse>(command);
        }

        public List<Lineup> GetLineups(IEnumerable<string> headends)
        {
            CheckAuthentication();
            var command = new AuthenticatedCommand() { 
                randhash = randhash, 
                @object = "lineups", 
                action = "get", 
                request = headends,
                api = ApiVersion,
            };
            var srcLineups = PostZip<dynamic>(command, FixLineup);
            var lineups = new List<Lineup>();

            
            foreach (var srcLineup in srcLineups)
            {
                var lineup = new Lineup()
                {
                    headend = srcLineup.headend,
                    name = srcLineup.name,
                    location = srcLineup.location,
                    deviceTypes = new List<DeviceType>()
                };

                lineup.stations = MyJson.Deserialize<List<Station>>(srcLineup.stationID.ToString());

                foreach (var devicetype in srcLineup.deviceTypes)
                {
                    var mapsJson = srcLineup[devicetype.Value].map.ToString();
                    var maps = MyJson.Deserialize<List<ChannelMap>>(mapsJson);
                    var deviceType = new DeviceType() { name = devicetype.Value, map = maps };
                    lineup.deviceTypes.Add(deviceType);

                    foreach (var map in maps)
                    {
                        map.deviceType = deviceType;
                        var station = lineup.stations.SingleOrDefault(station1 => station1.stationID == map.stationID);
                        if (station == null)
                        {
                            Console.WriteLine("stationid:" + map.stationID + " not found in stations for " + lineup.name + " channel:" + map.channel);
                            continue;
                        }
                        map.station = station;
                        station.channelMaps.Add(map);
                    }
                }

                lineups.Add(lineup);
            }
            return lineups;
        }

        public void FixLineup(ref dynamic lineup, string filename)
        {
            var headend = filename.Split(new char[] { '.' })[0];
            headend = headend.Replace("PC_", "PC:");
            lineup.headend = headend;
        }

        public IEnumerable<Schedule> GetSchedules(IEnumerable<int> stationIDs)
        {
            CheckAuthentication();

            var command = new AuthenticatedCommand()
            {
                randhash = randhash,
                @object = "schedules",
                action = "get",
                request = stationIDs,
                api = ApiVersion,
            };

            var programLists = PostZip<List<Program>>(command);

            var stationEnumerator = stationIDs.GetEnumerator();
            foreach (var programList in programLists) {
                stationEnumerator.MoveNext();
                Debug.WriteLine("Yielding schedule for stationid:" + stationEnumerator.Current);
                yield return new Schedule() { stationId = stationEnumerator.Current, programs = programList };
            }
        }

        public IEnumerable<Program> FillInProgramInfo(IEnumerable<Program> programs)
        {
            var command = new AuthenticatedCommand()
            {
                randhash = randhash,
                @object = "programs",
                action = "get",
                request = programs.Select(program => program.programID).Distinct(),
                api = ApiVersion,
            };

            var programInfos = PostZip<Program.Info>(command).ToList();

            foreach (var program in programs)
            {
                program.info = programInfos.Single(pi => pi.programID == program.programID);
                Debug.WriteLine("Filled in " + program + " with " + program.info);
                yield return program;
            }
        }

        public IEnumerable<MetaData> GetMetadata()
        {
            var command = new AuthenticatedCommand()
            {
                randhash = randhash,
                @object = "metadata",
                action = "get",
                api = ApiVersion,
            };

            var metaDatas = PostZip<MetaData>(command).ToList();

            return metaDatas;
        }

        //private static string GetZippedJson(string url, string txtFilename)
        //{
        //    var client = new HttpClient();
        //    var stream = client.GetStreamAsync(url).Result;
        //    var zipStream = new ZipInputStream(stream);

        //    string s = "[";
        //    var reader = new StreamReader(zipStream);
        //    var zipEntry = zipStream.GetNextEntry();
        //    while (zipEntry != null)
        //    {
        //        if (zipEntry.IsFile && zipEntry.Name != "serverID.txt")
        //        {
        //            s += "{\"filename\":\"" + zipEntry.Name + "\",\"content\":" + reader.ReadToEnd() + "},";
        //        }
        //        zipEntry = zipStream.GetNextEntry();
        //    }

        //    s += "]";

        //    return s;
        //}

        public delegate void FixupZip<T>(ref T obj, string filename);

        private static IEnumerable<T> GetZippedItems<T>(string url, FixupZip<T> fixup = null)
        {
            var client = new HttpClient();
            var stream = client.GetStreamAsync(url).Result;
            var zipStream = new ZipInputStream(stream);

            string s = "[";
            var reader = new StreamReader(zipStream);
            var zipEntry = zipStream.GetNextEntry();
            while (zipEntry != null)
            {
                if (zipEntry.IsFile && zipEntry.Name != "serverID.txt")
                {
                    s = reader.ReadToEnd();

                    Debug.WriteLine(zipEntry.Name + ": " + s);

                    T result;
                    if (typeof(T) == typeof(string))
                    {
                        result = (T)(object)s;
                    }
                    else
                    {
                        result = MyJson.Deserialize<T>(s);
                    }

                    if (fixup != null)
                    {
                        fixup(ref result, zipEntry.Name);
                    }
                    yield return result;
                }
                zipEntry = zipStream.GetNextEntry();
            }
        }

        private IEnumerable<T> PostZip<T>(object obj, FixupZip<T> fixup = null)
        {
            var s = Post<ZipResponse>(obj);
            return GetZippedItems<T>(s.URL, fixup);
        }

        private T Post<T>(object obj)
        {
            var request = MyJson.Serialize(obj);

            var content = new FormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>() { 
                        new KeyValuePair<string, string>("request", request),
                        new KeyValuePair<string, string>("submit", "submit")
                    }
                );

            Debug.WriteLine("Request:  " + request);
            var response = client.PostAsync(Page, content).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Response: " + response.ReasonPhrase + "(status code: " + response.StatusCode + ")");
            }

            var s = response.Content.ReadAsStringAsync().Result;
            Debug.WriteLine("Response: " + s);

            if (typeof(T) == typeof(String))
            {
                return (T)(object)s;
            }
            else
            {
                var result = MyJson.Deserialize<T>(s);
                if (result is Response)
                {
                    var resp = (result as Response);
                    switch (resp.response)
                    {
                        case "OK":
                            return result;
                        case "OFFLINE":
                            throw new ResponseException("Server Offline.  Please do not reconnect for 1 hour.", resp);
                        default:
                            throw new ResponseException(resp);
                    };
                }
                return result;
            }
        }
    }

    public class Authentication //: JsonObject
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Command //: JsonObject
    {
        public object request;
        public string @object;
        public string action;
        public int api;
    }

    public class AuthenticatedCommand : Command
    {
        public string randhash;
    }
}
