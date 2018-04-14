using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCalendar.Domain
{
    public interface IServiceMeeting
    {
        List<Meeting> GetMeetings(string nameEmp, string department, DateTime start, DateTime end);

        string SaveUpdateMeeting(Meeting meeting);
        string DeleteMeeting(string eventId);
    }
}
