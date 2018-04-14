using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using NetCalendar.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


using colorName = System.Drawing.Color;
using dataMeeting = NetCalendar.Domain.Meeting;
using googleEvent = Google.Apis.Calendar.v3.Data.Event;
          
namespace NetCalendar.DataLayer
{
    public class GoogleService
    {
        CalendarService service;

        private Dictionary<string, string> GoogleColors;

       
        public GoogleService(string path_dir, INotificationService notificationService)
        {
            service = GetService(path_dir);
            
            GoogleColors = new Dictionary<string, string>()
            {
                {"1", "CornflowerBlue" },
                {"2", "Green" },
                {"3", "DarkOrchid" },
                {"4", "HotPink" },
                {"5", "Goldenrod" },
                {"6", "Orange" },
                {"7", "Blue" },
                {"8", "DimGray" },
                {"9", "DarkSlateBlue" },
                {"10", "SeaGreen" },
                {"11", "Red" }
            };
        }

        private CalendarService GetService(string path_dir)
        {
            UserCredential credential = null;
            string secret_file;
            string error;

            ClientSecrets clSec;
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Calendar Net";
            
            
            secret_file = Path.Combine(path_dir, "client_secret.json");
            string credentialPath = Path.Combine(path_dir, "credentials");

            using (var stream = new FileStream(secret_file, FileMode.Open, FileAccess.Read))
            {
                clSec = GoogleClientSecrets.Load(stream).Secrets;
            }

            try
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clSec,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credentialPath, true)).Result;

                service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            }
            catch (Exception ex)
            {

                error = ex.Message;
                while (ex.InnerException != null)
                {
                    string temp = ex.Message;
                    ex = ex.InnerException;
                    if (!temp.Equals(ex.Message))
                        error += ex.Message;
                }
                throw new InvalidOperationException(error);
            }            

            return service;
        }

        public List<dataMeeting> GetMeetings(string nameEmployee, string department, DateTime start, DateTime end)
        {
            List<dataMeeting> meetings = new List<dataMeeting>();
            List<googleEvent> googleEventlist = new List<googleEvent>();
            Predicate<googleEvent> predicate;

            Events googleEvents = GetEvents(start, end);

            if (googleEvents.Items != null)
            {
                if (nameEmployee != null)     //рабочие смены для определенного сотрудника
                {
                    predicate = new Predicate<googleEvent>(_event =>
                                                                _event.Description.Equals(department) &&
                                                                IsConsistEmployee(_event, nameEmployee)
                                                           );
                }
                else                          //рабочие смены за весь департамент
                {
                    predicate = new Predicate<googleEvent>(_event => _event.Description.Equals(department));
                }
           
                googleEventlist = googleEvents.Items.ToList().FindAll(predicate);

                foreach (googleEvent _event in googleEventlist)
                {
                    meetings.Add(ToDataMeeting(_event));
                }
            }            
            return meetings;
        }


        public string SaveUpdateEvent(dataMeeting meeting)
        {
            googleEvent gEvent = GetEvent(meeting.GoogleEventId);
            googleEvent newGoogleEvent = new googleEvent();
            ToGoogleEvent(meeting, newGoogleEvent);

            if (gEvent != null)
            {
                DelEvent(gEvent.Id);
            }

            EventsResource.InsertRequest insertRequest = service.Events.Insert(newGoogleEvent, "primary");
            newGoogleEvent = insertRequest.Execute();

            return newGoogleEvent.Summary;
        }


        public string DelEvent(string eventId)
        {
            EventsResource.DeleteRequest request = service.Events.Delete("primary", eventId);
            string temp = request.Execute();
            return temp;
        }


        private Events GetEvents(DateTime start, DateTime end)
        {
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = start;
            request.TimeMax = end;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 500;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            try
            {
                Events googleEvents = request.Execute();
                return googleEvents;
            }
            catch
            {
                return null;
            }
           
        }


        private googleEvent GetEvent(string eventId)
        {
            if (eventId == null)
                return null;

            EventsResource.GetRequest request = service.Events.Get("primary", eventId);

            try
            {
                return request.Execute();
            }
            catch
            {
                return null;
            }
        }


        void ToGoogleEvent(dataMeeting dataMeeting, googleEvent googleEvent)
        {
            #region Приводим время клиента в соответствие с часовым поясом сервера
            TimeSpan timeSpanServer = DateTimeOffset.Now.Offset;
            TimeSpan timeSpanClient = new TimeSpan(dataMeeting.Offset, 0, 0);
            DateTime start;
            DateTime end;
            
            if (timeSpanServer.Hours != timeSpanClient.Hours *(-1))
            {
                start = dataMeeting.Start.Add(timeSpanClient);
                end = dataMeeting.End.Add(timeSpanClient);
            }
            else
            {
                start = dataMeeting.Start;
                end = dataMeeting.End;
            }
            #endregion

            

            //название департамента в description
            googleEvent.Description = dataMeeting.Department;

            googleEvent.ColorId = GoogleColors.First(keyValueColor => keyValueColor.Value == dataMeeting.ThemeColor).Key;
            googleEvent.Start = new EventDateTime() { DateTime = start };
            googleEvent.End = new EventDateTime() { DateTime = end };
            googleEvent.Location = dataMeeting.DestLat + " " + dataMeeting.DestLong;

            //заголовок и адрес храним в поле summary
            googleEvent.Summary = dataMeeting.Subject + " @" + dataMeeting.Adress;

            //прикрепляем к событию сотрудников --- Attendees
            googleEvent.Attendees = AddAttendees(dataMeeting);
        }


        private List<EventAttendee> AddAttendees(dataMeeting dataMeeting)
        {
            List<EventAttendee> attendees = new List<EventAttendee>();

            if (dataMeeting.Employees.Count > 0)
            {
               EventAttendee attendy;
                int number = 0;

                foreach (Employee e in dataMeeting.Employees)
                {
                    attendy = new EventAttendee();
                    attendy.Comment = e.Name;

                    if (e.Email == null || e.Email.Equals(""))
                    {
                        attendy.Email = number.ToString() + "notHaveEmail@not.com";     //если нет email, передаем фиктивный
                        number++;
                    }
                    else
                    {
                        if (attendees.Count > 0)
                        {
                            foreach (EventAttendee existAtt in attendees)
                            {
                                if (existAtt.Email == e.Email)               //нельзя передавать дубликат, заменяем на фиктивный
                                {
                                    e.Email = number.ToString() + "notHaveEmail@not.com";
                                    number++;
                                }
                            }
                        }
                        attendy.Email = e.Email;
                    }
                    attendees.Add(attendy);
                }
            }
            return attendees;
        }

        private dataMeeting ToDataMeeting(googleEvent googleEvent)
        {

            dataMeeting meeting = new dataMeeting();

            string geolat = googleEvent.Location != null ? googleEvent.Location.Split(' ')[0] : "";
            string geolongt = googleEvent.Location != null ? googleEvent.Location.Split(' ')[1] : "";
            string[] subjectAndAdress = googleEvent.Summary.Split('@');

            meeting.Department = googleEvent.Description;
            meeting.Subject = subjectAndAdress[0];

            meeting.Start = ((DateTime)googleEvent.Start.DateTime).ToUniversalTime();
            meeting.End = ((DateTime)googleEvent.End.DateTime).ToUniversalTime();
           

            meeting.ThemeColor = GoogleColors.First(keyValueColor => keyValueColor.Key == googleEvent.ColorId).Value;
            meeting.GoogleEventId = googleEvent.Id;
            meeting.DestLat = geolat;
            meeting.DestLong = geolongt;


            if (subjectAndAdress.Count() > 1)
            {
                meeting.Adress = subjectAndAdress[1];
            }
            else
            {
                meeting.Adress = " ";
            }
            
            //получаем прикрепленных сотрудников
            if (googleEvent.Attendees!=null)
            {
                meeting.Employees = new List<Employee>();
                foreach(EventAttendee att in googleEvent.Attendees)
                {
                    Employee temp = new Employee()
                    {
                        Department = googleEvent.Description,
                        Email = att.Email,
                        Name = att.Comment                   
                    };
                }
            }
            return meeting;
        }


        bool IsConsistEmployee(googleEvent @event, string nameemployee)
        {
            if(@event.Attendees!=null)
            {
                foreach(EventAttendee att in @event.Attendees)
                {
                    if (att.Comment.Equals(nameemployee))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
