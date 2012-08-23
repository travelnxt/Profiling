using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Profiling;
using Tavisca.Services.Profiling.Contract;

namespace Tavisca.Services.Profiling
{
    public interface IProfilerStorage
    {
        void Save(Debug debugResult,  bool async);
        Contract.Debug GetTimings(string transactionId);
        List<Contract.SqlTiming> GetSqlTimings(string transactionId, string timingId);
    }
}
