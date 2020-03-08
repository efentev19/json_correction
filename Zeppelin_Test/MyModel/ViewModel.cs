using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zeppelin_Test.MyModel
{
    public class ViewModel
    {
        public List<Person> workers { get; set; }
        public List<Event> events { get; set; }
        public List<Event> eventsNew { get; set; }
    }
}