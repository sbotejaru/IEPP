using IEPP.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEPP.Models
{
    public class HistoryItem
    {
        public string BrowseDate { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Domain { get; set; }
        public string HostName { get; set; }

        public HistoryItemContainer ToContainer()
        {
            return new HistoryItemContainer()
            {
                Title = Title,
                Url = Url,
                BrowseDate = BrowseDate,
                Domain = Domain,
                HostName = HostName
            };
        }

        public static bool operator == (HistoryItem h1, HistoryItem h2)
        {
            if (ReferenceEquals(h1, h2))
                return true;

            if (ReferenceEquals(h2, null))
                return false;

            if (ReferenceEquals(h1, null))
                return false;

            return h1.BrowseDate == h2.BrowseDate && h1.Title == h2.Title && h1.Url == h2.Url;
        }

        public static bool operator !=(HistoryItem h1, HistoryItem h2) => !(h1 == h2);
    }
}
