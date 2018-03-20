using NetCalendar.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace NetCalendar.DataLayer
{
    public class NotificationService : INotificationService
    {
        public async Task SendInvitesAsync(Event dataEvent)
        {
            if (dataEvent.Employees.Count > 0)
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("netcalendarmanager@gmail.com", "90909090vv");

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;

                foreach (Employee empl in dataEvent.Employees)
                {
                    if (empl.Email == null || empl.Email.Split('@')[1].Equals("not.com") || empl.Email.Equals("netcalendarmanager@gmail.com"))
                        continue;

                    string message = CreateMessage(dataEvent, empl.Name);

                    MailMessage mail = new MailMessage("netcalendarmanager@gmail.com",
                        empl.Email,
                        "New event in NetCalendar " + dataEvent.Start.ToString(),
                        message);

                    mail.IsBodyHtml = false;
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    await smtpClient.SendMailAsync(mail);
                }
            }
        }


        private string CreateMessage(Event dataEvent, string name)
        {
            //double _offset = (double)(dataEvent.Offset * (-1));
            //DateTime tempS = GetDateTime(gEvent.Start);
            //tempS = tempS.AddHours(_offset);

            //DateTime tempE = GetDateTime(gEvent.End);
            //tempE = tempE.AddHours(_offset);

            return String.Format(
                "Уважаемый {0}!!!\nНовое событие --- {1}  адрес: {2}\nНачало --- {3}\nКонец --- {4}\nПосмотреть на сайте:  {5} ",
                name,
                dataEvent.Subject,
                dataEvent.Adress,
                dataEvent.Start.ToString(),
                dataEvent.End.ToString(),
                "https://netcalendarweb.azurewebsites.net/"
                );
        }
    }
}
