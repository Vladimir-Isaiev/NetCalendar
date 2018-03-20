using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCalendar.Domain
{
    public interface IServiceEvent
    {
        List<Event> GetEvents(string nameEmp, string department, DateTime start, DateTime end);

        Task<string> SaveUpdateEventAsync(Event ev);
        string DeleteEvent(string eventId);
    }
}
