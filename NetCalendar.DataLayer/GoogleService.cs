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
using dataEvent = NetCalendar.Domain.Event;
using googleEvent = Google.Apis.Calendar.v3.Data.Event;
          
namespace NetCalendar.DataLayer
{
    public class GoogleService
    {
        INotificationService notificationService;
        CalendarService service;

        public GoogleService(string path_dir, INotificationService notificationService)
        {
            this.notificationService = notificationService;
            UserCredential credential = null;
            string _path_dir;
            string secret_file;
            string error;

            ClientSecrets clSec;
            string[] Scopes = { CalendarService.Scope.Calendar };
            string ApplicationName = "Calendar Net";
            



            _path_dir = path_dir;
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
            }
            catch(Exception ex)
            {

                error = ex.Message;
                while (ex.InnerException != null)
                {
                    string temp = ex.Message;
                    ex = ex.InnerException;
                    if (!temp.Equals(ex.Message))
                        error += ex.Message;
                }
            }

            service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }



        public List<dataEvent> GetEvents(string nameEmployee, string department, DateTime start, DateTime end)
        {
            List<dataEvent> events = new List<dataEvent>();
            List<googleEvent> googleEventlist = new List<googleEvent>();
            Predicate<googleEvent> predicate;

            Events googleEvents = GetEvents(start, end);

            if (googleEvents.Items != null)
            {
                if (nameEmployee != null)
                {
                    predicate = new Predicate<googleEvent>(ev =>
                                                                ev.Description.Equals(department) &&
                                                                IsConsistEmployee(ev, nameEmployee)
                                                           );
                }
                else
                {
                    predicate = new Predicate<googleEvent>(ev => ev.Description.Equals(department));
                }
           
                googleEventlist = googleEvents.Items.ToList().FindAll(predicate);

                foreach (googleEvent ev in googleEventlist)
                {
                    events.Add(ToDataEvent(ev));
                }
            }            
            return events;
        }


        public async Task<string> SaveUpdateEventAsync(dataEvent dataEv)
        {
            googleEvent gEvent = GetEvent(dataEv.GoogleEventId);
            googleEvent newGEvent = new googleEvent();
            ToGoogleEvent(dataEv, newGEvent);

            if (gEvent != null)
            {
                DelEvent(gEvent.Id);
            }

            EventsResource.InsertRequest insertRequest = service.Events.Insert(newGEvent, "primary");
            newGEvent = insertRequest.Execute();

            await notificationService.SendInvitesAsync(dataEv);
            return newGEvent.Summary;
        }


        public string DelEvent(string eventId)
        {
            EventsResource.DeleteRequest request = service.Events.Delete("primary", eventId);
            string temp = request.Execute();
            return temp;
        }


        CalendarListEntry GetCalendar()
        {
            CalendarListResource.GetRequest getRequest = service.CalendarList.Get("primary");
            CalendarListEntry calendar; 

            try
            {
                calendar = getRequest.Execute();
                return calendar;
            }
            catch
            {
                return null;
            }
        }



        Events GetEvents(DateTime start, DateTime end)
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
                googleEvent @event = request.Execute();
                return @event;
            }
            catch
            {
                return null;
            }
        }


               
        string GetCol(string colId)
        {
            string color;

            switch (colId)
            {
                case "1":
                        color = colorName.CornflowerBlue.Name;
                    break;
                case "2":
                    color = colorName.Green.Name;
                    break;
                case "3":
                    color = colorName.DarkOrchid.Name;
                    break;
                case "4":
                    color = colorName.HotPink.Name;
                    break;
                case "5":
                    color = colorName.Goldenrod.Name;
                    break;
                case "6":
                    color = colorName.Orange.Name;
                    break;
                case "8":
                    color = colorName.DimGray.Name;
                    break;
                case "9":
                    color = colorName.DarkSlateBlue.Name;
                    break;
                case "10":
                    color = colorName.SeaGreen.Name;
                    break;
                case "11":
                    color = colorName.Red.Name;
                    break;
                default:
                    color = colorName.Blue.Name;
                    break;
            }


            return color;
        }


        string GetColId(string col)
        {
            string color;

            switch (col)
            {
                case "CornflowerBlue":
                    color = "1";
                    break;
                case "Green":
                    color = "2";
                    break;
                case "DarkOrchid":
                    color = "3";
                    break;
                case "HotPink":
                    color = "4";
                    break;
                case "Goldenrod":
                    color = "5";
                    break;
                case "Orange":
                    color = "6";
                    break;
                case "DimGray":
                    color = "8";
                    break;
                case "DarkSlateBlue":
                    color = "9";
                    break;
                case "SeaGreen":
                    color = "10";
                    break;
                case "Red":
                    color = "11";
                    break;
                default:
                    color = "7";
                    break;
            }
            return color;
        }


        void ToGoogleEvent(dataEvent dataEvent, googleEvent googleEvent)
        {
            #region Приводим время клиента в соответствие с часовым поясом сервера
            TimeSpan timeSpanServer = DateTimeOffset.Now.Offset;
            TimeSpan timeSpanClient = new TimeSpan(dataEvent.Offset, 0, 0);
            DateTime start;
            DateTime end;
            
            if (timeSpanServer.Hours != timeSpanClient.Hours)
            {
                start = dataEvent.Start.Add(timeSpanClient);
                end = dataEvent.End.Add(timeSpanClient);
            }
            else
            {
                start = dataEvent.Start;
                end = dataEvent.End;
            }
            #endregion

            

            //название департамента в description
            googleEvent.Description = dataEvent.Department;

            googleEvent.ColorId = GetColId(dataEvent.ThemeColor);            
            googleEvent.Start = new EventDateTime() { DateTime = start };
            googleEvent.End = new EventDateTime() { DateTime = end };
            googleEvent.Location = dataEvent.DestLat + " " + dataEvent.DestLong;

            //заголовок и адрес храним в поле summary
            googleEvent.Summary = dataEvent.Subject + " @" + dataEvent.Adress;

            #region прикрепляем к событию сотрудников --- Attendees
            if (dataEvent.Employees.Count>0)
            {
                googleEvent.Attendees = new List<EventAttendee>();
                EventAttendee att;
                int number = 0;

                foreach(Employee e in dataEvent.Employees)
                {
                    att = new EventAttendee();
                    att.Comment = e.Name;

                    if (e.Email == null || e.Email.Equals(""))
                    {
                        att.Email = number.ToString() + "notHaveEmail@not.com";//email-заглушка
                        number++;
                    }
                    else
                    {
                        if(googleEvent.Attendees.Count>0)
                        {
                            foreach (EventAttendee existAtt in googleEvent.Attendees)
                            {
                                if(existAtt.Email == e.Email)
                                {
                                    //email-заглушка для повторяющихся email
                                    e.Email = number.ToString() + "notHaveEmail@not.com";
                                    number++;
                                }
                            }
                        }
                            att.Email = e.Email;
                    }
                    googleEvent.Attendees.Add(att);
                }
            }
            #endregion
        }


        dataEvent ToDataEvent(googleEvent googleEvent)
        {
            
            dataEvent @event = new dataEvent();
            string geolat = googleEvent.Location != null ? googleEvent.Location.Split(' ')[0] : "";
            string geolongt = googleEvent.Location != null ? googleEvent.Location.Split(' ')[1] : "";
            string[] subjectAndAdress = googleEvent.Summary.Split('@');

            @event.Department = googleEvent.Description;
            @event.Subject = subjectAndAdress[0];

            @event.Start = GetDateTime(googleEvent.Start);
            @event.End = GetDateTime(googleEvent.End);

            @event.ThemeColor = GetCol(googleEvent.ColorId);
            @event.GoogleEventId = googleEvent.Id;
            @event.DestLat = geolat;
            @event.DestLong = geolongt;

            
            if(subjectAndAdress.Count()>1)
                @event.Adress = subjectAndAdress[1];
            else
                @event.Adress = " ";


            //получаем прикрепленных сотрудников
            if (googleEvent.Attendees!=null)
            {
                @event.Employees = new List<Employee>();
                foreach(EventAttendee att in googleEvent.Attendees)
                {
                    Employee temp = new Employee()
                    {
                        Department = googleEvent.Description,
                        Email = att.Email,
                        Name = att.Comment                   
                    };

                    @event.Employees.Add(temp);
                }
            }
            return @event;
        }


        DateTime GetDateTime(EventDateTime eventDateTime)
        {
            DateTime dt;
           
            if(eventDateTime.DateTime!=null)
            {
                dt = (DateTime)eventDateTime.DateTime;
            }
            else
            {
                int y = Int32.Parse(eventDateTime.Date.Substring(0, 4));
                int m = Int32.Parse(eventDateTime.Date.Substring(5, 2));
                int d = Int32.Parse(eventDateTime.Date.Substring(8, 2));

                dt = new DateTime(y, m, d);
            }
            return dt;
        }


        bool IsConsistEmployee(googleEvent @event, string nameemployee)
        {
            bool answer = false;

            if(@event.Attendees!=null)
            {
                foreach(EventAttendee att in @event.Attendees)
                {
                    if (att.Comment.Equals(nameemployee))
                    {
                        answer = true;
                        return answer;
                    }
                }
            }
            return answer;
        }
    }
}
