using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleware
{
    public class Logger
    {
        //private static readonly string BaseDirectory = ConfigurationManager.AppSettings["TCPDataLogPath"]
        //?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"); // Fallback if not configured
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;


        public static void LogMessage(string message, string type)
        {
            try
            {
                // Create the month-based folder
                string TCPDataLoggerFolder = Path.Combine(BaseDirectory, "Log");
                if (!Directory.Exists(TCPDataLoggerFolder))
                {
                    Directory.CreateDirectory(TCPDataLoggerFolder);
                }
                string monthFolder = Path.Combine(TCPDataLoggerFolder, DateTime.Now.ToString("yyyy-MM"));
                if (!Directory.Exists(monthFolder))
                {
                    Directory.CreateDirectory(monthFolder);
                }

                // Create the day-based folder within the month folder
                string dayFolder = Path.Combine(monthFolder, DateTime.Now.ToString("dd"));
                if (!Directory.Exists(dayFolder))
                {
                    Directory.CreateDirectory(dayFolder);
                }
                string hourFolder = Path.Combine(dayFolder, DateTime.Now.ToString("HH"));
                if (!Directory.Exists(dayFolder))
                {
                    Directory.CreateDirectory(dayFolder);
                }

                // Create the hour-based log file within the day folder
                string logFileName = type;
                string logFilePath = Path.Combine(hourFolder, logFileName);

                // Write log data
                File.AppendAllText(logFilePath, $"{DateTime.Now:mm:ss.fff} {message}\n");
            }
            catch
            {

            }
        }
    }
}
