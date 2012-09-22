using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Profiling;
using StackExchange.Profiling.Wcf;
using System.Threading;
using Tavisca.Services.Profiling.Contract;


namespace Tavisca.Services.Profiling
{
    sealed public class Profiler : IProfiler
    {
        private static volatile IProfiler _instance;
        private static object syncRoot = new object();
        
        public static IProfiler Instance
        {
            get {
                if ((_instance == null) == true) {
                    lock (syncRoot) {
                        if ((_instance == null) == true)
                            _instance = new Profiler();
                    }
                }
                return _instance;
            }
        }

        private Profiler() { }

        void IProfiler.Start(ProfilerEnvironment profilerEnvironment, string name)
        {
            SetUpProfileProvider(profilerEnvironment, name);
            MiniProfiler.Start();
            Instance.AddMetaData("managedthreadid", Thread.CurrentThread.ManagedThreadId.ToString());
        }

        public IDisposable Step(string caption)
        {
            return Step(caption, Contract.ProfileLevel.Verbose);
        }

        public IDisposable Step(string caption, Contract.ProfileLevel level)
        {
            var profiler = MiniProfiler.Current;
            if ((profiler == null) == false)
            {
                var step = profiler.Step(caption, 
                                        level == Contract.ProfileLevel.Info ? 
                                                            StackExchange.Profiling.ProfileLevel.Info 
                                                            : StackExchange.Profiling.ProfileLevel.Verbose);
                Instance.AddData("managedthreadid", Thread.CurrentThread.ManagedThreadId.ToString());
                return step;
            }
            return null;
        }

        void IProfiler.Stop(bool save, bool async)
        {
            MiniProfiler.Stop(!save);
            if ((save == true) == true)
            {
                var profiler = MiniProfiler.Current;
                if ((profiler == null) == false)
                {
                    profiler.Save(async);
                };
            }
        }

        void IProfiler.AddData(string key, string value)
        {
            if ((string.IsNullOrWhiteSpace(key)) == true)
                throw new ArgumentNullException("key can't be null");

            var profiler = MiniProfiler.Current;
            if ((profiler == null) == false)
                profiler.AddData(key.ToLower(), value);
        }

        void IProfiler.AddMetaData(string key, string value)
        {
            if ((string.IsNullOrWhiteSpace(key)) == true)
                throw new ArgumentNullException("key can't be null");

            var profiler = MiniProfiler.Current;
            if ((profiler == null) == false)
            {
                if ((profiler.Root == null) == false)
                    profiler.Root.AddKeyValue(key.ToLower(), value);
            }
        }

        string IProfiler.GetData(string key)
        {
            if ((string.IsNullOrWhiteSpace(key)) == true)
                throw new ArgumentNullException("key can't be null");

            string result = null;

            var profiler = MiniProfiler.Current;
            if ((profiler == null) == false)
            {
                if (profiler.Head != null)
                {
                    if ((profiler.Head.KeyValues == null) == false)
                        result = profiler.Head.KeyValues[key.ToLower()];
                }
            }

            return result;
        }

        string IProfiler.GetMetaData(string key)
        {
            if ((string.IsNullOrWhiteSpace(key)) == true)
                throw new ArgumentNullException("key can't be null");

            string result = null;

            var profiler = MiniProfiler.Current;
            if ((profiler == null) == false)
            {
                if ((profiler.Root == null) == false)
                {
                    if ((profiler.Root.KeyValues == null) == false)
                        result = profiler.Root.KeyValues[key.ToLower()];
                }
            }
            return result;
        }

        private string _user;
        string IProfiler.User {
            get
            {
                if (_user == null)
                    _user = "Unknown";

                return _user;
            }
            set { _user = value; }
        }

        bool IProfiler.Enabled { get; set; }

        private string _transactionId;
        string IProfiler.TransactionId { 
            get {
                if (string.IsNullOrWhiteSpace(_transactionId))
                    _transactionId = Guid.NewGuid().ToString();

                return _transactionId;
            }
            set { _transactionId = value; } 
        }

        private static void SetUpProfileProvider(ProfilerEnvironment profilerEnvironment, string name)
        {
            if (profilerEnvironment == ProfilerEnvironment.Desktop)
                MiniProfiler.Settings.ProfilerProvider = new DesktopAppProfilerProvider(name);
            else if (profilerEnvironment == ProfilerEnvironment.Restfull)
                MiniProfiler.Settings.ProfilerProvider = new RestRequestProfilerProvider();
            else
                MiniProfiler.Settings.ProfilerProvider = new WcfRequestProfilerProvider();
        }
    }

    public class StepTiming : StackExchange.Profiling.Timing
    {
        public StepTiming(StackExchange.Profiling.Timing timing) : base(MiniProfiler.Current, timing.ParentTiming, timing.Name) { }
    }
}
