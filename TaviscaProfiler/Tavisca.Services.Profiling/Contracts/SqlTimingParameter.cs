using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Tavisca.Services.Profiling.Contract
{
    [DataContract]
    public class SqlTimingParameter
    {
        [DataMember]
        public string SqlTimingId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string DbType { get; set; }

        [DataMember]
        public int Size { get; set; }
    }
}
