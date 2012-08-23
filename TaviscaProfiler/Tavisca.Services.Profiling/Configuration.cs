using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Tavisca.Services.Profiling
{
    class Configuration
    {
        public static bool DebugEnabledGlobal
        {
            get {
                var debugEnabledStr = ConfigurationManager.AppSettings["debug.enabled.global"];
                bool debugEnabled = false;
                bool.TryParse(debugEnabledStr, out debugEnabled);
                return debugEnabled;
            }
        }

        public static string StorageObjectString
        {
            get {
                return ConfigurationManager.AppSettings["debug.storage"] ?? "Tavisca.Services.Profiling.SqlServerProfilerStorage, Tavisca.Services.Profiling";
            }
        }
    }
}
