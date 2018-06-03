using System;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;
using BattleTech;
using BattleTech.UI;
using DG.Tweening;
using Harmony;
using TMPro;

namespace AddYearToTimeline
{
    public static class Control
    {
        internal static Settings Settings = new Settings();

        #region Start
        public static void Start(string directory, string json)
        {
            var harmony = HarmonyInstance.Create("ProperTimeline.Control");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            // Read Settings
            try
            {
                Settings.LoadSettings(Settings);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
        #endregion

        #region Harmony Patch
        [HarmonyPatch(typeof(SGEventPanel), "SetEvent")]
        public static class SetEventSGEventPanelBattleTechPatch
        {
            public static void Postfix(SGEventPanel __instance)
            {
                try
                {
                    int daysPassed = UnityGameInstance.BattleTechGame.Simulation.DaysPassed;
                    string message = GetTimelineDate(daysPassed);
                    TextMeshProUGUI timePassedText = (TextMeshProUGUI)ReflectionHelper.GetPrivateField(__instance, "eventTime");
                    timePassedText.text = message;
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
        [HarmonyPatch(typeof(SGTimePlayPause), "SetDay")]
        public static class SetDaySGTimePlayPauseBattleTechPatch
        {
            //public static void Prefix()
            //{
            //    return;
            //}

            public static void Postfix(SGTimePlayPause __instance, int daysPassed)
            {
                try
                {
                    string message = GetTimelineDate(daysPassed);
                    TextMeshProUGUI timePassedText = (TextMeshProUGUI)ReflectionHelper.GetPrivateField(__instance, "timePassedText");
                    timePassedText.text = message;

                    WwiseManager.PostEvent<AudioEventList_ui>(AudioEventList_ui.ui_sim_travel_ping_play, WwiseManager.GlobalAudioObject, null, null);
                    int day = daysPassed % 7 + 1;
                    for (int i = 0; i < 7; i++)
                    {
                        List<DOTweenAnimation> DayPips = (List<DOTweenAnimation>)ReflectionHelper.GetPrivateField(__instance, "DayPips");
                        if (i < day)
                        {
                            DayPips[i].DOPlayForwardById("fadeIn");
                        }
                        else
                        {
                            DayPips[i].DOPlayBackwardsById("fadeIn");
                        }
                    }
                    __instance.CheckForLaunchVisbility();
                }
                catch (Exception e)
                {
                    Logger.LogError(e);
                }
            }
        }
        #endregion

        public static string GetTimelineDate(int daysPassed)
        {
            string message = string.Empty;
            try
            {
                int daysperyear = Settings.WeeksPerYear * 7;
                int day = daysPassed % 7 + 1;
                if (Settings.UseTerranTime)
                {
                    DateTime date = new DateTime(Settings.StartingYear, 1, 1);
                    date = date.AddDays(daysPassed);
                    string year = date.ToString("yyyy", CultureInfo.InvariantCulture);
                    string month = date.ToString("MMMM", CultureInfo.InvariantCulture);
                    string daysuffix = GetDaySuffix(date.Day);
                    message = string.Format("{0,4} {1,9} {2,2}{3,2}", year, month, date.Day, daysuffix);
                }
                else if (Settings.UseHBSFormat)
                {
                    decimal yearpassed = Math.Truncate((decimal)(daysPassed / daysperyear));
                    decimal year = Settings.StartingYear + yearpassed;
                    decimal week = Math.Truncate((decimal)(((daysPassed / 7) % Settings.WeeksPerYear) + 1));
                    message = string.Format("{0} Week {1}  Day {2}", year, week, day);
                }
                return message;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                return message;
            }
        }
        public static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }
    }
}
