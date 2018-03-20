using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCalendar.Domain
{
    public interface INotificationService
    {
         Task SendInvitesAsync(Event gEvent);
    }
}
