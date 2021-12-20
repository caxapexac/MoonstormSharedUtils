﻿using BepInEx.Logging;
using System.Runtime.CompilerServices;

namespace Moonstorm
{
    internal class MSULog
    {
        internal static ManualLogSource logger = null;

        internal MSULog(ManualLogSource logger_)
        {
            logger = logger_;
        }

        internal static void LogD(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogDebug(logString(data, i, member));
        }
        internal static void LogE(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogError(logString(data, i, member));
        }
        internal static void LogF(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            logger.LogFatal(logString(data, i, member));
        }
        internal static void LogI(object data)
        {
            logger.LogInfo(data);
        }
        internal static void LogM(object data)
        {
            logger.LogMessage(data);
        }
        internal static void LogW(object data)
        {
            logger.LogWarning(data);
        }

        private static string logString(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
        {
            return string.Format("{0} :: Line: {1}, Method {2}", data, i, member);
        }
    }
}
