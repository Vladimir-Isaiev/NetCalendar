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
    public class GoogleRepo : IServiceMeeting
    {
        protected GoogleService googleService;

        public GoogleRepo(GoogleService googleService)
        {
            this.googleService = googleService;
        }


        public List<Meeting> GetMeetings(string nameEmp, string department, DateTime start, DateTime end)
        {
            return googleService.GetMeetings(nameEmp, department, start, end);
        }


        public string SaveUpdateMeeting(Meeting meeting)
        {
            return googleService.SaveUpdateEvent(meeting);
        }

        public string DeleteMeeting(string eventId)
        {
            return googleService.DelEvent(eventId);
        }
    }
}
