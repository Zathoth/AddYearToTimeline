using System;
using System.IO;
using HBS.Util;

namespace AddYearToTimeline
{
    public class Settings
    {
        public bool UseTerranTime = false;
        public bool UseHBSFormat = true;
        public int StartingYear = 3025;
        public int WeeksPerYear = 52;

        public bool Logging = true;

        public void LoadSettings<T>(T settings) where T : Settings
        {
            try
            {
                string SettingsPath = "mods/AddYearToTimeline/Settings.json";

                if (!File.Exists(SettingsPath))
                {
                    return;
                }

                using (var reader = new StreamReader(SettingsPath))
                {
                    var json = reader.ReadToEnd();
                    JSONSerializationUtility.FromJSON(settings, json);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }
}
