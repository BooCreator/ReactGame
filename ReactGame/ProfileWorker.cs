using System;
using System.Collections.Generic;
using System.IO;

namespace ReactGame
{
    public static class ProfileWorker
    {
        public static List<String> LoadProfiles()
        {
            var Res = new List<String>();
            if (XMLSaver.Load(out Dictionary<string, string> Table, $"profiles.xml", "", out string Error))
            {
                foreach (var Item in Table)
                    Res.Add(Item.Key);
            }
            return Res;
        }
        public static Boolean AddProfile(String ProfileName)
            => XMLSaver.Save(new Dictionary<string, Dictionary<string, string>> { { ProfileName, null } }, "profiles.xml", out string Error); 
        public static Boolean RemoveProfile(String ProfileName) 
            => XMLSaver.Remove(ProfileName, "profiles.xml", out string Error);

        public static Double[] LoadProfileData(String Profile, String ValueName)
        {
            if (XMLSaver.Load(out Dictionary<string, string> Table, $"Save\\{Profile}\\data.xml", ValueName, out string Error))
            {
                if(Table.Count > 0)
                    if(Double.TryParse(Table["points"], out double points))
                    {
                        if (Double.TryParse(Table["time"], out double time))
                        {
                            return new double[2] { points, time };
                        }
                    }
            }
            return null;
        }
        public static Boolean SaveProfileData(String Profile, String ValueName, Double[] Values)
        {
            Dictionary<String, Dictionary<String, String>> Data = new Dictionary<string, Dictionary<string, string>>();
            Data.Add(ValueName, new Dictionary<string, string>() {
                { "points", Values[0].ToString() },
                { "time", Values[1].ToString() },
            });
            if (!Directory.Exists("Save"))
                Directory.CreateDirectory("Save");
            if (!Directory.Exists($"Save\\{Profile}"))
                Directory.CreateDirectory($"Save\\{Profile}");
            return XMLSaver.Save(Data, $"Save\\{Profile}\\data.xml", out string Error);
        }
    }
}
