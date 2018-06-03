using System;
using System.IO;

namespace AddYearToTimeline
{
    public class Logger
    {
        public static void LogError(Exception ex)
        {
            string filePath = "mods/AddYearToTimeline/Log.txt";
            (new FileInfo(filePath)).Directory.Create();
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(DateTime.Now.ToString() + " : Message : " + ex.Message + Environment.NewLine + "StackTrace : " + ex.StackTrace);
            }
        }

        public static void LogLine(String line)
        {
            string filePath = "mods/AddYearToTimeline/Log.txt";
            (new FileInfo(filePath)).Directory.Create();
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(DateTime.Now.ToString() + " : " + line);
            }
        }
    }
   
}
