using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SExchange = StackExchange.Profiling;
using TProfiling = Tavisca.Services.Profiling.Contract;
using System.Xml.Linq;


namespace Tavisca.Services.Profiling
{
    public static class MiniProfilerExtensions
    {
        public static void Save(this SExchange.MiniProfiler miniProfiler, bool async)
        {
            if ((miniProfiler == null) == true) return;
            var profilerStorage = Activator.CreateInstance(Type.GetType(Configuration.StorageObjectString)) as IProfilerStorage;
            if ((profilerStorage == null) == false)
            {
                var debug = miniProfiler.ToDebugResultSet();
                try {
                    profilerStorage.Save(debug, async);
                }
                catch { /*suppress the exception*/ }
            }
        }

        public static TProfiling.Debug ToDebugResultSet(this SExchange.MiniProfiler miniProfiler)
        {
            TProfiling.Debug debug = null;
            if ((miniProfiler == null) == true) return debug;

            debug = BuildDebug(miniProfiler, debug);
            GetHeadInfo(miniProfiler, debug);

            return debug;
        }

        private static void GetHeadInfo(SExchange.MiniProfiler miniProfiler, TProfiling.Debug debug)
        {
            var mRoot = miniProfiler.Root;
            if ((mRoot == null) == false)
            {
                GetHeadInfo(debug, mRoot, miniProfiler);
                GetSqlTimingInfo(debug.Head, mRoot);
                GetTimingInfo(debug.Head.Children, mRoot);
            }
        }

        private static void GetHeadInfo(TProfiling.Debug debug, SExchange.Timing mRoot, SExchange.MiniProfiler miniProfiler)
        {
            debug.Head = new TProfiling.Timing() { 
                Id = mRoot.Id.ToString(),
                IsRoot = mRoot.IsRoot,
                Name = mRoot.Name,
                Order = 0,
                ParentId = Guid.Empty.ToString(),
                Start = mRoot.StartMilliseconds,
                Duration = mRoot.DurationMilliseconds.HasValue ? mRoot.DurationMilliseconds.Value : 0,
                DurationWithOutChildren = mRoot.DurationWithoutChildrenMilliseconds,
                SqlTimingDuration = mRoot.SqlTimingsDurationMilliseconds,
                ExecutedNonQueries = mRoot.ExecutedNonQueries,
                ExecutedReaders = mRoot.ExecutedReaders,
                ExecutedScalers = mRoot.ExecutedScalars,
                KeyValues = mRoot.KeyValues.MetadataToXml(),
                ManagedThreadId = ((mRoot.KeyValues != null) && (mRoot.KeyValues.ContainsKey("managedthreadid"))) 
                ? int.Parse(mRoot.KeyValues["managedthreadid"]) : -1
            };
        }

        private static void GetSqlTimingInfo(TProfiling.Timing timing, SExchange.Timing mTiming)
        {
            if ((mTiming.HasSqlTimings == true) == true)
            {
                for (int sqlTindex = 0; sqlTindex < mTiming.SqlTimings.Count; sqlTindex++)
                {
                    SExchange.SqlTiming mSqlTiming;
                    TProfiling.SqlTiming sqlTiming;
                    BuildSqlTiming(mTiming, sqlTindex, out mSqlTiming, out sqlTiming);
                    GetSqlParamterInfo(mSqlTiming, sqlTiming);
                    timing.SqlTimings.Add(sqlTiming);
                }
            }
        }

        private static void BuildSqlTiming(SExchange.Timing mTiming, int sqlTindex, out SExchange.SqlTiming mSqlTiming, out TProfiling.SqlTiming sqlTiming)
        {
            mSqlTiming = mTiming.SqlTimings[sqlTindex];
            sqlTiming = new TProfiling.SqlTiming()
            {
                CommandString = mSqlTiming.FormattedCommandString,
                DurationMilliseconds = mSqlTiming.DurationMilliseconds,
                ExecuteType = mSqlTiming.ExecuteType.ToString(),
                FirstFetchDurationMilliseconds = mSqlTiming.FirstFetchDurationMilliseconds,
                Id = mSqlTiming.Id.ToString(),
                Order = sqlTindex,
                StartMilliseconds = mSqlTiming.StartMilliseconds,
                TimingId = mTiming.Id.ToString()
            };
        }

        private static void GetSqlParamterInfo(SExchange.SqlTiming mSqlTiming, TProfiling.SqlTiming sqlTiming)
        {
            if ((mSqlTiming.Parameters != null) && (mSqlTiming.Parameters.Count > 0))
            {
                for (int sqlpIndex = 0; sqlpIndex < mSqlTiming.Parameters.Count; sqlpIndex++)
                {
                    var sqlParameter = BuildSqlParameter(mSqlTiming, sqlTiming, sqlpIndex);
                    sqlTiming.Parameters.Add(sqlParameter);
                }
            }
        }

        private static TProfiling.SqlTimingParameter BuildSqlParameter(SExchange.SqlTiming mSqlTiming, TProfiling.SqlTiming sqlTiming, int sqlpIndex)
        {
            var mSqlParameter = mSqlTiming.Parameters[sqlpIndex];
            var sqlParameter = new TProfiling.SqlTimingParameter()
            {
                DbType = mSqlParameter.DbType,
                Name = mSqlParameter.Name,
                Size = mSqlParameter.Size,
                Value = mSqlParameter.Value,
                SqlTimingId = sqlTiming.Id,
            };
            return sqlParameter;
        }

        private static void GetTimingInfo(List<TProfiling.Timing> children, SExchange.Timing parentTiming)
        {
            if ((parentTiming.Children != null) && (parentTiming.Children.Count > 0))
            {
                for (int timingIndex = 0; timingIndex < parentTiming.Children.Count; timingIndex++)
                {
                    SExchange.Timing mTiming;
                    TProfiling.Timing timing;
                    BuildTiming(parentTiming, timingIndex, out mTiming, out timing);

                    GetSqlTimingInfo(timing, mTiming);
                    GetTimingInfo(timing.Children, mTiming);
                    children.Add(timing);
                }
            }
        }

        private static void BuildTiming(SExchange.Timing parentTiming, int timingIndex, out SExchange.Timing mTiming, out TProfiling.Timing timing)
        {
            mTiming = parentTiming.Children[timingIndex];
            timing = new TProfiling.Timing()
            {
                Order = timingIndex,
                Id = mTiming.Id.ToString(),
                KeyValues = mTiming.KeyValues.DataToXml(),
                Name = mTiming.Name,
                Duration = mTiming.DurationMilliseconds.HasValue ? mTiming.DurationMilliseconds.Value : 0,
                DurationWithOutChildren = mTiming.DurationWithoutChildrenMilliseconds,
                SqlTimingDuration = mTiming.SqlTimingsDurationMilliseconds,
                ExecutedNonQueries = mTiming.ExecutedNonQueries,
                ExecutedReaders = mTiming.ExecutedReaders,
                ExecutedScalers = mTiming.ExecutedScalars,
                IsRoot = mTiming.IsRoot,
                ParentId = mTiming.ParentTimingId.HasValue ? mTiming.ParentTimingId.Value.ToString() : Guid.Empty.ToString(),
                Start = mTiming.StartMilliseconds,
                ManagedThreadId = ((mTiming.KeyValues != null) && (mTiming.KeyValues.ContainsKey("managedthreadid"))) ? int.Parse(mTiming.KeyValues["managedthreadid"]) : -1
            };
        }

        private static TProfiling.Debug BuildDebug(SExchange.MiniProfiler miniProfiler, TProfiling.Debug debug)
        {
            debug = new TProfiling.Debug()
            {
                Id = miniProfiler.Id.ToString(),
                Level = miniProfiler.Level.ToString(),
                MachineName = miniProfiler.MachineName,
                Name = miniProfiler.ToString(),
                RequestUrl = ((miniProfiler.Root != null) &&
                                    (miniProfiler.Root.KeyValues != null) &&
                                    miniProfiler.Root.KeyValues.ContainsKey("requesturl"))
                                    ? miniProfiler.Root.KeyValues["requesturl"]
                                    : string.Empty,
                Method = ((miniProfiler.Root != null) &&
                                    (miniProfiler.Root.KeyValues != null) &&
                                    miniProfiler.Root.KeyValues.ContainsKey("method"))
                                    ? miniProfiler.Root.KeyValues["method"]
                                    : string.Empty,
                Code = ((miniProfiler.Root != null) &&
                                    (miniProfiler.Root.KeyValues != null) &&
                                    miniProfiler.Root.KeyValues.ContainsKey("code"))
                                    ? miniProfiler.Root.KeyValues["code"]
                                    : string.Empty,
                TransactionId = Profiler.Instance.TransactionId,
                StartedUtc = miniProfiler.Started,
                UserName = Profiler.Instance.User
            };
            return debug;
        }

        public static string MetadataToXml(this Dictionary<string, string> keyValues)
        {
            var xkeyvalues = new XElement("keyvalues");
            if ((keyValues == null) == false) {
                foreach (var keyValue in keyValues.Where(x => !(new List<string>
                        { "managedthreadid", "requesturl", "method", "code"}).Contains(x.Key))) {
                    xkeyvalues.Add(new XElement(keyValue.Key, keyValue.Value ?? string.Empty));
                }
            }
            return xkeyvalues.ToString(SaveOptions.DisableFormatting);
        }

        public static string DataToXml(this IDictionary<string, string> keyValues)
        {
            var xkeyvalues = new XElement("keyvalues");
            if ((keyValues == null) == false) {
                foreach (var keyValue in keyValues.Where( x => x.Key != "managedthreadid")) {
                    xkeyvalues.Add(new XElement(keyValue.Key, keyValue.Value ?? string.Empty));
                }
            }
            return xkeyvalues.ToString(SaveOptions.DisableFormatting);
        }
    }
}
