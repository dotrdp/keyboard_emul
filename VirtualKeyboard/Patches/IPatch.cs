using System;
using HarmonyLib;
using StardewModdingAPI;

namespace VirtualKeyboard.Patches
{
    /// <summary>
    /// Base class for all Harmony patches following TASMod pattern
    /// </summary>
    public abstract class IPatch
    {
        /// <summary>
        /// Static reference to the mod's monitor for logging
        /// </summary>
        public static IMonitor? Monitor { get; set; }

        /// <summary>
        /// The name of this patch for logging purposes
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Apply this patch using the provided Harmony instance
        /// </summary>
        /// <param name="harmony">The Harmony instance to use for patching</param>
        public abstract void Patch(Harmony harmony);

        /// <summary>
        /// Log a trace message
        /// </summary>
        /// <param name="trace">The trace message</param>
        public static void Trace(string trace)
        {
            Monitor?.Log(trace, LogLevel.Trace);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="warning">The warning message</param>
        public static void Warn(string warning)
        {
            Monitor?.Log(warning, LogLevel.Warn);
        }

        /// <summary>
        /// Log an alert message
        /// </summary>
        /// <param name="alert">The alert message</param>
        public static void Alert(string alert)
        {
            Monitor?.Log(alert, LogLevel.Alert);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="error">The error message</param>
        public static void Error(string error)
        {
            Monitor?.Log(error, LogLevel.Error);
        }

        /// <summary>
        /// Log an info message
        /// </summary>
        /// <param name="info">The info message</param>
        public static void Info(string info)
        {
            Monitor?.Log(info, LogLevel.Info);
        }
    }
}
