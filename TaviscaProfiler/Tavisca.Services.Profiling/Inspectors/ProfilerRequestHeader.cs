using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Tavisca.Services.Profiling
{
    [DataContract]
    public class ProfilerRequestHeader
    {
        public const string HeaderName = "ProfilerRequestHeader";
        public const string HeaderNamespace = "http://www.tavisca.com/profiling/services/2012/3";

        [DataMember]
        public string ProfilerId { get; set; }

        [DataMember]
        public string TransactionId { get; set; }

        [DataMember]
        public string ParentTimingId { get; set; }

        [DataMember]
        public bool DebugEnabled { get; set; }

        [DataMember]
        public string User { get; set; }
    }
}
