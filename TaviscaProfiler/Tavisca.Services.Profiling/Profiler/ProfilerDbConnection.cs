using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace Tavisca.Services.Profiling
{
    public class ProfilerDbConnection : ProfiledDbConnection
    {
        public ProfilerDbConnection(DbConnection connection) : base(connection, MiniProfiler.Current)
        {   }
    }
}
