using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tavisca.Services.Profiling.Contract;

namespace Tavisca.Services.Profiling
{
    public class SqlServerProfilerStorage : IProfilerStorage
    {
        private const string UNKNOWN = "Unknown";

        #region Save
        
        public void Save(Debug debugResult, bool async)
        {
            Action<Debug> action = (debug) => {
                using (var profilerDataContext = new ProfilerDbDataContext())
                {
                    var transactionId = Guid.Empty;
                    Guid.TryParse(debug.TransactionId, out transactionId);

                    profilerDataContext.InsertProfilerDebug(Guid.Parse(debug.Id), debug.Name, debug.Method,
                        debug.RequestUrl, debug.MachineName, debug.UserName, debug.StartedUtc,
                        transactionId, debug.Code, debug.Level);

                    if (debug.Head != null)
                    {
                        var timing = debug.Head;
                        InsertTimings(profilerDataContext, transactionId, timing);
                    }
                }
            };

            if (RequiredToSave() == true)
            {
                if (async)
                    action.BeginInvoke(debugResult, null, null);
                else
                    action.Invoke(debugResult);
            }
            
        }

        private bool RequiredToSave()
        {
            if (Profiler.Instance.Enabled)
                return true;
            else
                return Configuration.DebugEnabledGlobal;
        }

        private static void InsertTimings(ProfilerDbDataContext profilerDataContext, Guid transactionId, Timing timing)
        {
            var timingId = Guid.Empty;
            Guid.TryParse(timing.Id, out timingId);
            profilerDataContext.InsertProfilerTiming(timingId, transactionId, timing.IsRoot, timing.Name, timing.KeyValues,
                    timing.Duration, timing.DurationWithOutChildren, timing.Start, Guid.Parse(timing.ParentId), timing.Order,
                    timing.SqlTimingDuration, timing.ExecutedScalers, timing.ExecutedNonQueries, timing.ExecutedReaders, timing.ManagedThreadId);


            foreach (var sqlTiming in timing.SqlTimings)
            {
                var sqlTimingId = Guid.Empty;
                Guid.TryParse(sqlTiming.Id, out sqlTimingId);
                profilerDataContext.InsertProfilerSqlTiming(sqlTiming.ExecuteType, sqlTiming.CommandString, sqlTiming.StartMilliseconds,
                    sqlTiming.DurationMilliseconds, sqlTiming.FirstFetchDurationMilliseconds, timingId, transactionId, sqlTiming.Order, sqlTimingId);
                foreach (var sqlParameter in sqlTiming.Parameters)
                {
                    profilerDataContext.InsertProfilerSqlParameter(sqlTimingId, sqlParameter.Name, sqlParameter.Value,
                        sqlParameter.DbType, sqlParameter.Size);
                }
            }

            if (timing.Children.Count > 0)
            {
                foreach (var childTiming in timing.Children)
                {
                    InsertTimings(profilerDataContext, transactionId, childTiming);
                }
            }

        }

        #endregion

        #region Timings

        public Debug GetTimings(string transactionId)
        {
            var transactionIdGuid = Guid.Parse(transactionId);
            var profilerDataContext = new ProfilerDbDataContext();
            var traces = profilerDataContext.GetTraceAndTimings(transactionIdGuid);
            
            Debug result = new Debug();

            List<Timing> allTimings = new List<Timing>();
            foreach (var trace in traces)
            {
                if (trace.IsRoot.GetValueOrDefault() == true) {
                    BuildDebugAndRoot(trace, result);
                }
                else{ 
                    var timing = new Timing();
                    BuildTiming(trace, timing);
                    BuildSqlTimingCount(trace, timing);
                    allTimings.Add(timing);
                }
            }
            
            if(allTimings.Count > 0)
                MapChildren(result.Head, allTimings);
            return result;
        }

        private void MapChildren(Timing parent, List<Timing> allTimings)
        {
            var children = allTimings.Where(x => parent.Id == x.ParentId);
            foreach (var child in children)
            {
                parent.Children.Add(child);
                MapChildren(child, allTimings);
            }
        }

        private void BuildDebugAndRoot(spGetTraceAndTimingsResult trace, Debug debug)
        {
            BuildDebug(trace, debug);
            BuildTiming(trace, debug.Head);
            BuildSqlTimingCount(trace, debug.Head);
        }

        private void BuildTiming(spGetTraceAndTimingsResult trace, Timing timing)
        {
            timing.Id = trace.TimingId.GetValueOrDefault(Guid.Empty).ToString();
            timing.Name = trace.TimingName ?? UNKNOWN;
            timing.IsRoot = trace.IsRoot.GetValueOrDefault();
            timing.Start = trace.Start.GetValueOrDefault();
            timing.Duration = trace.Duration.GetValueOrDefault();
            timing.DurationWithOutChildren = trace.DurationWithOutChildren.GetValueOrDefault();
            timing.KeyValues = trace.KeyValues ?? UNKNOWN;
            timing.ManagedThreadId = trace.ManagedThreadId.GetValueOrDefault();
            timing.Order = trace.ProfiledOrder.GetValueOrDefault();
            timing.ParentId = trace.ParentTimingId.GetValueOrDefault(Guid.Empty).ToString();
        }

        private void BuildSqlTimingCount(spGetTraceAndTimingsResult trace, Timing timing)
        {
            var sqlDuration = trace.SqlTimingDuration.GetValueOrDefault();
            var nonQueries = trace.ExecutedNonQueries.GetValueOrDefault();
            var readers = trace.ExecutedReaders.GetValueOrDefault();
            var scalars = trace.ExecutedScalers.GetValueOrDefault();
            timing.SqlTimingsCount = nonQueries + readers + scalars;
            timing.SqlTimingDuration = sqlDuration;
            timing.ExecutedNonQueries = nonQueries;
            timing.ExecutedReaders = readers;
            timing.ExecutedScalers = scalars;
        }

        private static void BuildDebug(spGetTraceAndTimingsResult trace, Debug debug)
        {
            debug.Id = trace.ProfilerId.ToString();
            debug.Code = trace.Code ?? UNKNOWN;
            debug.MachineName = trace.MachineName ?? UNKNOWN;
            debug.Method = trace.Method ?? UNKNOWN;
            debug.Name = trace.DebugName ?? UNKNOWN;
            debug.RequestUrl = trace.RequestUrl ?? UNKNOWN;
            debug.StartedUtc = trace.StartedUtc.GetValueOrDefault();
            debug.Level = trace.ProfileLevel ?? UNKNOWN;
            debug.UserName = trace.UserName ?? UNKNOWN;
            debug.TransactionId = trace.TransactionId.GetValueOrDefault(Guid.Empty).ToString();
            debug.Head = new Timing();
        }

        #endregion

        #region SqlTimings
        
        public List<SqlTiming> GetSqlTimings(string transactionId, string timingId)
        {
            List<SqlTiming> result = new List<SqlTiming>();
            var profilerDataContext = new ProfilerDbDataContext();

            var sqlResults = profilerDataContext.GetSqlTimings(Guid.Parse(timingId), Guid.Parse(transactionId));
            foreach (var sqlResult in sqlResults)
            {
                var preservedSqlResult = result.Find(x => x.Id == sqlResult.SqlTimingId.ToString());
                if ((preservedSqlResult == null) == true)
                {
                    preservedSqlResult = new SqlTiming();
                    preservedSqlResult.CommandString = sqlResult.CommandString ??  UNKNOWN;
                    preservedSqlResult.DurationMilliseconds = sqlResult.DurationMilliseconds.GetValueOrDefault();
                    preservedSqlResult.ExecuteType = sqlResult.ExecuteType ?? UNKNOWN;
                    preservedSqlResult.FirstFetchDurationMilliseconds = sqlResult.FirstFetchDurationMilliseconds.GetValueOrDefault();
                    preservedSqlResult.Id = sqlResult.SqlTimingId.ToString();
                    preservedSqlResult.Order = sqlResult.ProfiledOrder.GetValueOrDefault();
                    preservedSqlResult.StartMilliseconds = sqlResult.StartMilliseconds.GetValueOrDefault();
                    preservedSqlResult.TimingId = sqlResult.TimingId.ToString();
                    result.Add(preservedSqlResult);    
                }
                BuildSqlParameter(sqlResult, preservedSqlResult);
            }
            return result;
        }

        private static void BuildSqlParameter(spGetSqlTimingsResult sqlResult, SqlTiming preservedSqlResult)
        {
            var sqlParamter = new SqlTimingParameter();
            sqlParamter.DbType = sqlResult.DbType ?? UNKNOWN;
            sqlParamter.Name = sqlResult.Name ?? UNKNOWN;
            sqlParamter.Size = sqlResult.Size.GetValueOrDefault();
            sqlParamter.SqlTimingId = preservedSqlResult.Id;
            sqlParamter.Value = sqlResult.Value ?? UNKNOWN;
            preservedSqlResult.Parameters.Add(sqlParamter);
        }

        #endregion
    }
}
