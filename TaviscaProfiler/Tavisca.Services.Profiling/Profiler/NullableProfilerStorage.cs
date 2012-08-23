using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tavisca.Services.Profiling.Contract;

namespace Tavisca.Services.Profiling
{
    public class NullableProfilerStorage : IProfilerStorage
    {
        public void Save(Debug debugResult, bool async)
        {
            /*do nothing*/
        }

        public Debug GetTimings(string transactionId)
        {
            /*do nothing*/
            return null;
        }

        public List<SqlTiming> GetSqlTimings(string transactionId, string timingId)
        {
            /*do nothing*/
            return null;
        }
    }
}
