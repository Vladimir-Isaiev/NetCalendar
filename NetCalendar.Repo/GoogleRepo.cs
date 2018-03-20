using NetCalendar.DataLayer;
using NetCalendar.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NetCalendar.Repo
{
    public class GoogleRepo : IServiceEvent
    {
        protected GoogleService googleService;

        public GoogleRepo(GoogleService googleService)
        {
            this.googleService = googleService;
        }


        public List<Event> GetEvents(string nameEmp, string department, DateTime start, DateTime end)
        {
            List<Event> events = new List<Event>();

            events = googleService.GetEvents(nameEmp, department, start, end);
    
            return events;
        }


        public async Task<string> SaveUpdateEventAsync(Event ev)
        {
            return await googleService.SaveUpdateEventAsync(ev);
        }

        public string DeleteEvent(string eventId)
        {
            return googleService.DelEvent(eventId);
        }
    }
}
