using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NetCalendar.Domain
{
    public class NetCalendarService
    {
        private readonly IServiceMeeting repoMeeting;

        public NetCalendarService(IServiceMeeting serviceMeeting)
        {
            repoMeeting = serviceMeeting;
        }


        public List<Meeting> GetMeetingsOfEmployee(Employee employee, DateTime start, DateTime end)
        {
            DateTime _start = start;
            DateTime _end = end;
            if (start == null)
                _start = DateTime.Now.AddDays(-30);

            if(end == null)
                _end = DateTime.Now.AddDays(365);

            return repoMeeting.GetMeetings(employee.Name, employee.Department, _start, _end);
        }

        public List<Meeting> GetMeetingsOfDepartment(string department, DateTime start, DateTime end)
        {
            DateTime _start = start;
            DateTime _end = end;
            if (start == null)
                _start = DateTime.Now.AddDays(-30);

            if (end == null)
                _end = DateTime.Now.AddDays(365);

            return repoMeeting.GetMeetings(null, department, _start, _end);
        }



        public Task<string> SaveUpdateEventAsync(Meeting ev)
        {
           return repoMeeting.SaveUpdateMeetingAsync(ev);

        }

        public string DeleteMeeting(string idEvent, string department)
        {
            return repoMeeting.DeleteMeeting(idEvent);
        }

        public int GetMeetingsDuration(string department, DateTime start, DateTime end)
        {
            double sum = 0;
            TimeSpan timeSpan;
            List<Meeting> meetings = GetMeetingsOfDepartment(department, start, end);

            foreach(Meeting m in meetings)
            {
                timeSpan = m.End - m.Start;
                sum += timeSpan.TotalHours;
            }
                        
            return (int)sum;
        }



        public int GetMeetingsDuration(Employee emp, DateTime start, DateTime end)
        {
            double Meetingshours = 0;
            TimeSpan timeSpan;
            List<Meeting> events = GetMeetingsOfEmployee(emp, start, end);

            foreach (Meeting ev in events)
            {
                timeSpan = ev.End - ev.Start;
                Meetingshours += timeSpan.TotalHours;
            }

            return (int)Meetingshours;
        }
    }
}