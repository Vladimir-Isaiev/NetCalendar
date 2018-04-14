using NetCalendar.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace NetCalendar.Domain
{
    public class NotificationService : INotificationService
    {
        public async Task SendInvitesAsync(Meeting dataMeeting)
        {
            if (dataMeeting.Employees.Count > 0)
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("netcalendarmanager@gmail.com", "90909090vv");

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;

                foreach (Employee empl in dataMeeting.Employees)
                {
                    if (empl.Email == null || empl.Email.Split('@')[1].Equals("not.com") || empl.Email.Equals("netcalendarmanager@gmail.com"))
                        continue;

                    string message = CreateMessage(dataMeeting, empl.Name);

                    MailMessage mail = new MailMessage("netcalendarmanager@gmail.com",
                        empl.Email,
                        "New event in NetCalendar " + dataMeeting.Start.ToString(),
                        message);

                    mail.IsBodyHtml = false;
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    await smtpClient.SendMailAsync(mail);
                }
            }
        }


        private string CreateMessage(Meeting dataMeeting, string name)
        {
            return String.Format(
                "Уважаемый {0}!!!\nНовое событие --- {1}  адрес: {2}\nНачало --- {3}\nКонец --- {4}\nПосмотреть на сайте:  {5} ",
                name,
                dataMeeting.Subject,
                dataMeeting.Adress,
                dataMeeting.Start.ToString(),
                dataMeeting.End.ToString(),
                "https://netcalendarweb.azurewebsites.net/"
                );
        }
    }
}
