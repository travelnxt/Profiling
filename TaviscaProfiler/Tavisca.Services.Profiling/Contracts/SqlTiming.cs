using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Tavisca.Services.Profiling.Contract
{
    [DataContract]
    public class SqlTiming
    {
        public SqlTiming()
        {
            Parameters = new List<SqlTimingParameter>();
        }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string ExecuteType { get; set; }

        [DataMember]
        public string CommandString { get; set; }

        [DataMember]
        public decimal StartMilliseconds { get; set; }

        [DataMember]
        public decimal DurationMilliseconds { get; set; }

        [DataMember]
        public decimal FirstFetchDurationMilliseconds { get; set; }

        [DataMember]
        public List<SqlTimingParameter> Parameters { get; set; }

        [DataMember]
        public string TimingId { get; set; }

        [DataMember]
        public int Order { get; set; }
    }
}
