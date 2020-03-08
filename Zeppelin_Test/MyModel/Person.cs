using System;
using System.Collections.Generic;

namespace Zeppelin_Test.MyModel
{
    public class Person
    {
        public string Id { get; set; }
        public Dictionary<DateTime, List<WorkHours>> Hours { get ; set; }
    }

    public class WorkHours
    {
        public DateTime? Kommen { get; set; }
        public DateTime? Gehen { get; set; }
        public DateTime Day { get; set; }
        public bool[] Correction { get; set; }

        public WorkHours()
        {
            Correction = new bool[2];
        }            
    }
}