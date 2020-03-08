using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace Zeppelin_Test.MyModel
{
    public class Event
    {
        public string Rfid { get; set; }
        public DateTime EventTime { get; set; }
        public string EventSource { get; set; }
        public string EventType { get; set; }
        public bool Color { get; set; }
        public string Status { get; set; }
    }
}
