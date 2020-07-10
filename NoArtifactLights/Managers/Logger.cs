using System;
using System.IO;

namespace NoArtifactLights.Managers
{
    public class Logger
    {
        /// <summary>
        /// Is the logger instance alerady created once.
        /// </summary>
        private static bool Loaded;
        /// <summary>
        /// The level of logging.
        /// </summary>
        public enum LogLevel
        {
            /// <summary>
            /// Info.
            /// </summary>
            Info,
            /// <summary>
            /// A warning, something isn't normal but it is fine.
            /// </summary>
            Warning,
            /// <summary>
            /// A error, but it is not fatal error makes crash.
            /// </summary>
            Error,
            /// <summary>
            /// A fatal error, usually shut down the Landtory.
            /// </summary>
            Fatal
        }
        /// <summary>
        /// Creates an instance of logger.
        /// </summary>
        public Logger()
        {
            if (Loaded == false)
            {
                if (File.Exists("NAL.log"))
                {
                    File.WriteAllText("NAL.log", File.ReadAllText("NAL.log") + Environment.NewLine + "---------------------------------------"
                        + Environment.NewLine + "Log started: " + DateTime.Now.ToString() + Environment.NewLine);
                    Loaded = true;
                }
                else
                {
                    File.WriteAllText("NAL.log", "NoArtifactLights Mod Log File" + Environment.NewLine + "Log started: " + DateTime.Now.ToString() + Environment.NewLine);
                    Loaded = true;
                }
            }
        }
        /// <summary>
        /// Log text as LogLevel.
        /// </summary>
        /// <param name="text">Text that being logged.</param>
        /// <param name="level">Log Level.</param>
        public void Log(string text, LogLevel level = LogLevel.Info)
        {
            string target;
            switch (level)
            {
                case LogLevel.Info:
                    target = Environment.NewLine + "[" + DateTime.Now.ToString() + "] [INFO] " + text;
                    break;
                case LogLevel.Warning:
                    target = Environment.NewLine + "[" + DateTime.Now.ToString() + "] [WARN] " + text;
                    break;
                case LogLevel.Error:
                    target = Environment.NewLine + "*!!!* [" + DateTime.Now.ToString() + "] [ERROR] " + text;
                    break;
                case LogLevel.Fatal:
                    target = Environment.NewLine + "*!!!* [" + DateTime.Now.ToString() + "] [FATAL] " + text;
                    break;
                default:
                    target = Environment.NewLine + "[" + DateTime.Now.ToString() + "] [INFO] " + text;
                    break;
            }
            File.WriteAllText("NAL.log", File.ReadAllText("NAL.log") + target);
        }
        /// <summary>
        /// Log text as LogLevel, with submitted sender.
        /// </summary>
        /// <param name="text">Text that being logged.</param>
        /// <param name="sender">Sender that being submitted and logged.</param>
        /// <param name="level">Log Level.</param>
        public void Log(string text, string sender, LogLevel level = LogLevel.Info)
        {
            string target;
            switch (level)
            {
                case LogLevel.Info:
                    target = Environment.NewLine + "[" + DateTime.Now.ToString() + "] [" + sender + "] [INFO] " + text;
                    break;
                case LogLevel.Warning:
                    target = Environment.NewLine + "[" + DateTime.Now.ToString() + "] [" + sender + "] [WARN] " + text;
                    break;
                case LogLevel.Error:
                    target = Environment.NewLine + "*!!!* [" + DateTime.Now.ToString() + "] [" + sender + "] [ERROR] " + text;
                    break;
                case LogLevel.Fatal:
                    target = Environment.NewLine + "*!!!* [" + DateTime.Now.ToString() + "] [" + sender + "] [FATAL] " + text;
                    break;
                default:
                    target = Environment.NewLine + "[" + DateTime.Now.ToString() + "] [" + sender + "] [INFO] " + text;
                    break;
            }
            File.WriteAllText("NAL.log", File.ReadAllText("NAL.log") + target);
        }
    }
}
