using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Profiling;
using System.Data.Common;
using StackExchange.Profiling.Wcf;

namespace Tavisca.Services.Profiling
{
    public interface IProfiler
    {
        void Start(ProfilerEnvironment profilerEnvironment = ProfilerEnvironment.Soap, string name = "Unknown");
        void Stop(bool save = true, bool async = true);
        IDisposable Step(string caption);
        IDisposable Step(string caption, Contract.ProfileLevel level);
        void AddData(string key, string value);
        string GetData(string key);
        void AddMetaData(string key, string value);
        string GetMetaData(string key);
        string User { get; set; }
        bool Enabled { get; set; }
        string TransactionId { get; set; }
    }

    public enum ProfilerEnvironment
    {
        Soap,
        Restfull,
        Desktop
    }
}
