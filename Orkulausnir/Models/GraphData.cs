using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Orkulausnir.Models
{
    [DataContract]
    public class GraphData
    {
        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "dataPoints")]
        public IList<DataPoint> DataPoints { get; set; }
    }
}