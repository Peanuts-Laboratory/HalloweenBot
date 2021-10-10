using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Newtonsoft.Json.Linq;

namespace HalloweenBot
{
    public class Database
    {
        public static int CooldownTimeMinutes = MinutesToSeconds(1);
        public static int MinutesToSeconds(int Minutes) { return Minutes * 10; }

        public static string UserPath = Path.Combine(Directory.GetCurrentDirectory(), "Users");
        public static string ServerPath = Path.Combine(Directory.GetCurrentDirectory(), "Server");
        public static string CandyStats = Path.Combine(ServerPath, "CandyStats.json");

        /// <summary>
        /// On Launch, Sets up the Json Database used to store everything
        /// </summary>
        public static void StartDatabase()
        {
            string UserPath = Path.Combine(Directory.GetCurrentDirectory(), "Users");
            string ServerPath = Path.Combine(Directory.GetCurrentDirectory(), "Server");

            string CandyStats = Path.Combine(ServerPath, "CandyStats.json");
            if (!Directory.Exists(UserPath) || !Directory.Exists(ServerPath))
            {
                Directory.CreateDirectory(UserPath);
                Directory.CreateDirectory(ServerPath);
            }

            if (!File.Exists(CandyStats))
            {
                CandyStatsDB candy = new CandyStatsDB
                {
                    CandyReceieved = new int[] { 0, 0, 0, 0, 0 },
                    TotalMembersParticipating = 1
                };
                StreamWriter sw = File.CreateText(CandyStats);
                sw.WriteLine(JsonSerializer.Serialize(candy));
                sw.Close();
            }

            Console.WriteLine("Database Initalized");
        }

        /// <summary>
        /// Only ran if the user does not yet have an account
        /// </summary>
        /// <param name="user"></param>
        public static void CreateUser(IUser user)
        {
            string UserFile = Path.Combine(UserPath, $"{user.Id}.json");
            using (StreamWriter sw = new StreamWriter(UserFile))
            {
                UserDB db = new UserDB
                {
                    Nickname = user.Username,
                    UUID = user.Id,
                    Candy = new int[] { 0, 0, 0, 0, 0 },
                    TimesTrickOrTreating = 0,
                    ScaryMask = false,
                    ScaryCostume = false,
                    LastUsedTrickOrTreat = 0
                };

                sw.WriteLine(JsonSerializer.Serialize(db));
                sw.Close();
            }
        }

        /// <summary>
        /// Returns true if the user is registered, and vise versa
        /// </summary>
        public static bool UserRegistered(ulong ID)
        {
            if (File.Exists(Path.Combine(UserPath, $"{ID}.json")))
                return true;
            else
                return false;
        }

        public static JObject GetUserData(ulong ID)
        {
            var result = File.ReadAllText(Path.Combine(UserPath, $"{ID}.json"));
            JObject data = JObject.Parse(result);
            return data;
        }

        public static JObject GetServerData()
        {
            var result = File.ReadAllText(CandyStats);
            JObject data = JObject.Parse(result);
            return data;
        }

        public static void UpdateCandyAmount(int[] candyToBeAdded, ulong ID)
        {
            JObject data = GetUserData(ID);
            JObject server = GetServerData();
            int greenCandy = data["Candy"][0].ToObject<int>();
            int yellowCandy = data["Candy"][1].ToObject<int>();
            int redCandy = data["Candy"][2].ToObject<int>();
            int blueCandy = data["Candy"][3].ToObject<int>();
            int purpleCandy = data["Candy"][4].ToObject<int>();



            int addGreen = candyToBeAdded[0];
            int addYellow = candyToBeAdded[1];
            int addRed = candyToBeAdded[2];
            int addBlue = candyToBeAdded[3];
            int addPurple = candyToBeAdded[4];

            int[] newCandy = new int[] { greenCandy + addGreen, yellowCandy + addYellow, redCandy + addRed, blueCandy + addBlue, purpleCandy + addPurple };
            int[] serverCandy = new int[] {server["CandyReceieved"][0].ToObject<int>() + addGreen, server["CandyReceieved"][1].ToObject<int>() + addYellow,
                                           server["CandyReceieved"][2].ToObject<int>() + addRed, server["CandyReceieved"][3].ToObject<int>() + addBlue,
                                           server["CandyReceieved"][4].ToObject<int>() + addPurple };

            UserDB db = new UserDB
            {
                Nickname = (string)data["Nickname"],
                UUID = ID,
                Candy = newCandy,
                TimesTrickOrTreating = (int)data["TimesTrickOrTreating"],
                ScaryMask = (bool)data["ScaryMask"],
                ScaryCostume = (bool)data["ScaryCostume"],
                LastUsedTrickOrTreat = (long)data["LastUsedTrickOrTreat"]
            };

            CandyStatsDB stats = new CandyStatsDB
            {
                CandyReceieved = serverCandy,
                TotalMembersParticipating = (int)server["TotalMembersParticipating"]
            };

            File.Delete(Path.Combine(UserPath, $"{ID}.json"));
            using (StreamWriter sw = new StreamWriter(Path.Combine(UserPath, $"{ID}.json")))
            {
                sw.WriteLine(JsonSerializer.Serialize(db));
                sw.Close();
            }

            File.Delete(CandyStats);
            using (StreamWriter sw = new StreamWriter(CandyStats))
            {
                sw.WriteLine(JsonSerializer.Serialize(stats));
                sw.Close();
            }
        }

        public static bool OnCooldown(ulong ID)
        {
            JObject data = GetUserData(ID);
            long storedTime = data["LastUsedTrickOrTreat"].ToObject<long>();
            long timePassed = GetUnixTime() - storedTime;
            if (timePassed >= CooldownTimeMinutes)
                return false;
            else
                return true;
        }

        public static void SetUnixTime(ulong ID)
        {
            JObject data = GetUserData(ID);

            int[] candy = new int[] { data["Candy"][0].ToObject<int>(), data["Candy"][1].ToObject<int>(), data["Candy"][2].ToObject<int>(), data["Candy"][3].ToObject<int>(), data["Candy"][4].ToObject<int>() };
            UserDB db = new UserDB
            {
                Nickname = (string)data["Nickname"],
                UUID = ID,
                Candy = candy,
                TimesTrickOrTreating = (int)data["TimesTrickOrTreating"],
                ScaryMask = (bool)data["ScaryMask"],
                ScaryCostume = (bool)data["ScaryCostume"],
                LastUsedTrickOrTreat = GetUnixTime()
            };

            File.Delete(Path.Combine(UserPath, $"{ID}.json"));
            using (StreamWriter sw = new StreamWriter(Path.Combine(UserPath, $"{ID}.json")))
            {
                sw.WriteLine(JsonSerializer.Serialize(db));
                sw.Close();
            }
        }

        public static long GetUnixTime()
        {
            DateTime foo = DateTime.Now;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();
            return unixTime;
        }
    }

    public class UserDB
    {
        public string Nickname { get; set; }
        public ulong UUID { get; set; }
        public int[] Candy { get; set; }
        public int TimesTrickOrTreating { get; set; }
        public bool ScaryMask { get; set; }
        public bool ScaryCostume { get; set; }
        public long LastUsedTrickOrTreat { get; set; }
    }

    public class CandyStatsDB
    {
        public int TotalMembersParticipating { get; set; }
        public int[] CandyReceieved { get; set; }

    }
}
