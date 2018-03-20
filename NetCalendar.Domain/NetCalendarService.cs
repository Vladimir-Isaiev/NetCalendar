using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NetCalendar.Domain
{
    public class NetCalendarService
    {
        private readonly IServiceEvent repoEvent;

        public NetCalendarService(IServiceEvent repoEvent)
        {
            this.repoEvent = repoEvent;
        }


        public List<Event> GetEventsOfEmployee(Employee employee, DateTime start, DateTime end)
        {
            DateTime _start = start;
            DateTime _end = end;
            if (start == null)
                _start = DateTime.Now.AddDays(-30);

            if(end == null)
                _end = DateTime.Now.AddDays(365);

            return repoEvent.GetEvents(employee.Name, employee.Department, _start, _end);
        }

        public List<Event> GetEventsOfDepartment(string department, DateTime start, DateTime end)
        {
            DateTime _start = start;
            DateTime _end = end;
            if (start == null)
                _start = DateTime.Now.AddDays(-30);

            if (end == null)
                _end = DateTime.Now.AddDays(365);

            return repoEvent.GetEvents(null, department, _start, _end);
        }



        public Task<string> SaveUpdateEventAsync(Event ev)
        {
           return repoEvent.SaveUpdateEventAsync(ev);

        }

        public string DeleteEvent(string idEvent, string department)
        {
            return repoEvent.DeleteEvent(idEvent);
        }

        public int SumEventDepartment(string department, DateTime start, DateTime end)
        {
            int summ;
            double sum = 0;
            TimeSpan timeSpan;
            List<Event> events = GetEventsOfDepartment(department, start, end);

            foreach(Event ev in events)
            {
                timeSpan = ev.End - ev.Start;
                sum += timeSpan.TotalHours;
            }

            summ = (int)sum;
            return summ;
        }



        public int SumEventUser(Employee emp, DateTime start, DateTime end)
        {
            int summ;
            double sum = 0;
            TimeSpan timeSpan;
            List<Event> events = GetEventsOfEmployee(emp, start, end);

            foreach (Event ev in events)
            {
                timeSpan = ev.End - ev.Start;
                sum += timeSpan.TotalHours;
            }

            summ = (int)sum;
            return summ;
        }
    }
}