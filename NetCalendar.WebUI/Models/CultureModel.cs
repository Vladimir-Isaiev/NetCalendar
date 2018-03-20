using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetCalendar.WebUI.Models
{
    public class CultureModel
    {
        Dictionary<string, string> listCulture;
        public CultureModel()
        {
            listCulture = new Dictionary<string, string>()
            {
               { "en-US", "English" }, { "uk-UA", "Ukraine" }
            };
        }


        public string SelectedKeyCulture { get; set; } = "uk-UA";
        public Dictionary<string, string> ListCulture
        {
            get
            {
                return listCulture;
            }
            private set { listCulture = value; }
        }
    }
}