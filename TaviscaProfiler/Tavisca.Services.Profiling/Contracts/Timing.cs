using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Tavisca.Services.Profiling.Contract
{
    [DataContract]
    public class Timing
    {
        public Timing()
        {
            Children = new List<Timing>();
            SqlTimings = new List<SqlTiming>();
        }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public bool IsRoot { get; set; }

        [DataMember]
        public string ParentId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string KeyValues { get; set; }

        [DataMember]
        public decimal Duration { get; set; }

        [DataMember]
        public decimal DurationWithOutChildren { get; set; }

        [DataMember]
        public decimal Start { get; set; }

        [DataMember]
        public int Order { get; set; }

        [DataMember]
        public ProfileLevel Level { get; set; }

        [DataMember]
        public List<Timing> Children { get; set; }

        [DataMember]
        public List<SqlTiming> SqlTimings { get; set; }

        [DataMember]
        public decimal SqlTimingDuration { get; set; }

        [DataMember]
        public int ExecutedScalers { get; set; }

        [DataMember]
        public int ExecutedNonQueries { get; set; }
        
        [DataMember]
        public int ExecutedReaders { get; set; }

        [DataMember]
        public int ManagedThreadId { get; set; }

        [DataMember]
        public int SqlTimingsCount { get; set; }
    }
}
