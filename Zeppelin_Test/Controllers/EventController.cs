using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Zeppelin_Test.MyModel;

namespace Zeppelin_Test.Controllers
{
    public class EventController : Controller
    {
        // GET: Event/Correction
        public ActionResult Correction()
        {
            ViewModel model = new ViewModel();
            EventSort eventSort = new EventSort();
            List<Event> eventsCorrected = new List<Event>();

            // Path/Read/Deserialize JSON
            string jsonPath = Server.MapPath("~/Json/InsiteEventAp_EventStreamData.json");
            string jsonValues = System.IO.File.ReadAllText(jsonPath);
            List<Event> eventsOriginal = JsonConvert.DeserializeObject<List<Event>>(jsonValues);

            // Sort events ENTER/LEAVE and create list of workers with work time
            var persons = eventSort.Sort(eventsOriginal);

            // Check work hours
            persons = eventSort.EightHours(persons);

            // Convert Person Model to Event Model
            eventsCorrected = eventSort.ConvertPersonToEvent(persons);
            eventsCorrected.Sort((d1, d2) => DateTime.Compare(d1.EventTime, d2.EventTime));

            // Add elements to ViewModel
            model.workers = persons;
            model.events = eventsOriginal;
            model.eventsNew = eventsCorrected;

            return View(model);
        }       
    }
}