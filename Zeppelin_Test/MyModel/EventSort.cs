using System;
using System.Collections.Generic;
using System.Linq;

namespace Zeppelin_Test.MyModel
{
    public class EventSort
    {
        // Sort events ENTER/LEAVE and create list of workers with work time
        public List<Person> Sort(List<Event> events)
        {
            List<Person> persons = new List<Person>();

            foreach (var ev in events)
            {
                Person person = new Person() { Id = ev.Rfid };
                Dictionary<DateTime, List<WorkHours>> hours = new Dictionary<DateTime, List<WorkHours>>();

                // Add person in list
                if (persons.Find(x => x.Id == ev.Rfid) == null)
                {
                    persons.Add(person);
                }

                // Index of existing person
                var personExists = persons.Find(x => x.Id == ev.Rfid);
                int index = persons.IndexOf(personExists);

                // is "Hours" dictionary null?
                if (persons[index].Hours == null)
                {
                    hours.Add(ev.EventTime.Date, new List<WorkHours>());
                    persons[index].Hours = hours;
                }

                // does "Hours" not contain a key?
                if (!persons[index].Hours.ContainsKey(ev.EventTime.Date))
                {
                    persons[index].Hours.Add(ev.EventTime.Date, new List<WorkHours>());
                }

                // Get list element with specific key
                var workhours = persons[index].Hours[ev.EventTime.Date];

                // Event Status: ENTER or LEAVE
                switch (ev.EventType)
                {
                    case "ENTER":
                        // If list contains 0 elements
                        if (workhours.Count == 0)
                        {
                            persons[index].Hours[ev.EventTime.Date].Add(new WorkHours() { Kommen = ev.EventTime, Day = ev.EventTime.Date });
                        }
                        else
                        {
                            // Kommen is missing
                            if (workhours.Last().Kommen == null && workhours.Last().Gehen != null)
                            {
                                // Set value to Kommen
                                // e.g. Gehen: 16:30:00, EventTime for Kommen: 17:00:00 -> create new list element
                                if (workhours.Last().Gehen < ev.EventTime)
                                {
                                    persons[index].Hours[ev.EventTime.Date].Add(new WorkHours() { Kommen = ev.EventTime, Day = ev.EventTime.Date });
                                }
                                else
                                {
                                    workhours.Last().Kommen = ev.EventTime;
                                }
                            }
                            else
                            {
                                persons[index].Hours[ev.EventTime.Date].Add(new WorkHours() { Kommen = ev.EventTime, Day = ev.EventTime.Date });
                            }
                        }
                        break;
                    case "LEAVE":
                        // If list contains 0 elements
                        if (workhours.Count == 0)
                        {
                            persons[index].Hours[ev.EventTime.Date].Add(new WorkHours() { Gehen = ev.EventTime, Day = ev.EventTime.Date });
                        }
                        else
                        {
                            // Gehen is missing
                            if (workhours.Last().Gehen == null && workhours.Last().Kommen != null)
                            {
                                // Set value to Gehen
                                // e.g. Kommen: 16:30:00, EventTime for Gehen: 16:00:00 -> create new list element
                                if (workhours.Last().Kommen > ev.EventTime)
                                {
                                    persons[index].Hours[ev.EventTime.Date].Add(new WorkHours() { Gehen = ev.EventTime, Day = ev.EventTime.Date });
                                }
                                else
                                {
                                    workhours.Last().Gehen = ev.EventTime;
                                }
                            }
                            else
                            {
                                persons[index].Hours[ev.EventTime.Date].Add(new WorkHours() { Gehen = ev.EventTime, Day = ev.EventTime.Date });
                            }
                        }
                        break;
                }
            }

            return persons;
        }

        // Check work hours
        public List<Person> EightHours(List<Person> workers)
        {
            for (int i = 0; i < workers.Count; i++)
            {
                for (int j = 0; j < workers[i].Hours.Count; j++)
                {
                    int count = workers[i].Hours.ElementAt(j).Value.Count();
                    foreach (var item in workers[i].Hours.ElementAt(j).Value)
                    {
                        var eight = new TimeSpan(8, 0, 0);

                        // If list contains only one element of work hours
                        if (count == 1)
                        {
                            if (item.Kommen == null)
                            {
                                // Find Kommen from Gehen -> Kommen = Gehen - 8 hours
                                item.Kommen = item.Gehen.Value.Subtract(eight);

                                // If Kommen less then today's date -> Kommen = 00:00:00
                                if (item.Kommen < item.Day)
                                {
                                    item.Kommen = item.Day;
                                }
                                item.Correction[0] = true;
                            }
                            else if (item.Gehen == null)
                            {
                                // Find Gehen from Kommen -> Gehen = Kommen + 8 hours 
                                item.Gehen = item.Kommen.Value.AddHours(8);

                                // If Gehen more then (today's date + 23:59:59) -> Gehen = 23:59:59
                                if (item.Gehen > item.Day.Add(new TimeSpan(23, 59, 59)))
                                {
                                    item.Gehen = item.Day.Add(new TimeSpan(23, 59, 59));
                                }
                                item.Correction[1] = true;
                            }
                            else if (item.Gehen != null && item.Kommen != null)
                            {
                                // Check 8 hours of work
                                var time = item.Gehen.Value.Subtract(item.Kommen.Value);
                                if (time > new TimeSpan(8, 0, 0))
                                {
                                    // If more than 8 hours -> should be corrected
                                    item.Gehen = item.Kommen.Value.AddHours(8);
                                    item.Correction[1] = true;
                                }
                            }
                            else
                            {
                                item.Correction[0] = true;
                                item.Correction[1] = true;
                            }
                        }
                        // If list contains more than one element of work hours
                        else
                        {
                            if (item.Kommen == null)
                            {
                                TimeSpan work = new TimeSpan(8, 0, 0);
                                TimeSpan time = new TimeSpan();
                                TimeSpan end = new TimeSpan(0, 0, 0);
                                int? index = null;

                                // Count hours from one day (time)
                                foreach (var a in workers[i].Hours.ElementAt(j).Value)
                                {
                                    if (a.Gehen != null && a.Kommen != null)
                                    {
                                        time = time + a.Gehen.Value.Subtract(a.Kommen.Value);
                                        index = workers[i].Hours.ElementAt(j).Value.IndexOf(a);
                                    }
                                }

                                // If time more than 8 hours
                                if (time > work)
                                {
                                    time = work;
                                }
                                // Cant count work hours? e.g. Kommen: null, Gehen: 16:00:00    ;    Kommen: 18:00:00, Gehen: null
                                // Divide 8 hours / (list of work hours).Count
                                else if (time == new TimeSpan(0, 0, 0))
                                {
                                    int c = workers[i].Hours.ElementAt(j).Value.Count();
                                    time = new TimeSpan(eight.Ticks / c);
                                }

                                item.Kommen = item.Gehen.Value.Subtract(time);

                                if (item.Kommen < item.Day.Add(end))
                                {
                                    item.Kommen = item.Day;
                                }
                                else if (index != null)
                                {
                                    // If element1.Gehen >= element2.Kommen -> element2.Kommen = element1.Gehen + 15 min
                                    if (item.Kommen <= workers[i].Hours.ElementAt(j).Value[(int)index].Gehen)
                                    {
                                        item.Kommen = item.Kommen.Value.AddMinutes(15);
                                    }
                                }
                                else
                                {
                                    item.Kommen = item.Gehen.Value.Subtract(time);
                                }
                                item.Correction[0] = true;
                            }
                            else if (item.Gehen == null)
                            {
                                TimeSpan work = new TimeSpan(8, 0, 0);
                                TimeSpan time = new TimeSpan();
                                TimeSpan end = new TimeSpan(23, 59, 59);
                                foreach (var a in workers[i].Hours.ElementAt(j).Value)
                                {
                                    if (a.Gehen != null && a.Kommen != null)
                                    {
                                        time = time + a.Gehen.Value.Subtract(a.Kommen.Value);
                                    }
                                }

                                if (time > work)
                                {
                                    time = work;
                                }
                                time = work.Subtract(time);
                                item.Gehen = item.Kommen.Value.Add(time);

                                if (item.Gehen > item.Day.Add(end))
                                {
                                    item.Gehen = item.Day.Add(end);
                                }
                                item.Correction[1] = true;
                            }
                            else if (item.Gehen != null && item.Kommen != null && workers[i].Hours.ElementAt(j).Value.FindAll(x => x.Gehen != null && x.Kommen != null).Count > 1)
                            {
                                var list = workers[i].Hours.ElementAt(j).Value.FindAll(x => x.Gehen != null && x.Kommen != null);
                                TimeSpan time = new TimeSpan();
                                int? index = null;
                                foreach (var a in list)
                                {
                                    time = time + a.Gehen.Value.Subtract(a.Kommen.Value);
                                    index = workers[i].Hours.ElementAt(j).Value.IndexOf(a);
                                }

                                // Check 8 hours of work
                                if (time > eight)
                                {
                                    // If more than 8 hours -> should be corrected
                                    //time = new TimeSpan(8, 0, 0);
                                    var t = workers[i].Hours.ElementAt(j).Value[(int)index].Gehen.Value.Subtract(workers[i].Hours.ElementAt(j).Value[(int)index].Kommen.Value);
                                    time = time.Subtract(t);
                                    if(time <= eight)
                                    {
                                        workers[i].Hours.ElementAt(j).Value[(int)index].Gehen = workers[i].Hours.ElementAt(j).Value[(int)index].Kommen.Value.Add(eight.Subtract(time));
                                    }
                                    workers[i].Hours.ElementAt(j).Value[(int)index].Correction[1] = true;
                                }
                            }
                            else
                            {
                                
                            }
                        }
                    }
                }
            }

            return workers;
        }

        // Convert Person Model to Event Model
        public List<Event> ConvertPersonToEvent(List<Person> workers)
        {
            List<Event> events = new List<Event>();

            foreach (var item in workers)
            {
                foreach (var hour in item.Hours)
                {
                    foreach (var stunde in hour.Value)
                    {
                        try
                        {
                            Event evEnter = new Event()
                            {
                                Rfid = item.Id,
                                EventTime = (DateTime)stunde.Kommen,
                                EventSource = "SYS",
                                EventType = "ENTER",
                                Color = stunde.Correction[0]
                            };
                            Event evLeave = new Event()
                            {
                                Rfid = item.Id,
                                EventTime = (DateTime)stunde.Gehen,
                                EventSource = "SYS",
                                EventType = "LEAVE",
                                Color = stunde.Correction[1]
                            };

                            events.Add(evEnter);
                            events.Add(evLeave);
                        }
                        catch
                        {

                        }
                    }
                }
            }
            return events;
        }
    }
}