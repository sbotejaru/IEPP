using IEPP.Controls;
using Nager.PublicSuffix;
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
        // icon?

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
    }
}
