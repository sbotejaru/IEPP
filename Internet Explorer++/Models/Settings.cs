using IEPP.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEPP.Models
{
    public class Settings
    {
        public SearchEngine SearchEngine { get; set; }
        public bool BookmarkVisible { get; set; } = true;
        public string DownloadsFolder { get; set; }
        public bool AskWhereToDownload { get; set; } = true;
    }
}
