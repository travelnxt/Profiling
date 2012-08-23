using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Tavisca.Services.Profiling.Contract
{
    [DataContract]
    public class Debug
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Method { get; set; }

        [DataMember]
        public string RequestUrl { get; set; }

        [DataMember]
        public string MachineName { get; set; }

        [DataMember]
        public string Level { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public DateTime? StartedUtc { get; set; }

        [DataMember]
        public Timing Head { get; set; }

        [DataMember]
        public string TransactionId { get; set; }

        [DataMember]
        public string Code { get; set; }
    }
}
