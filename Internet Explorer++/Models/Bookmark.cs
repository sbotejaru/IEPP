using IEPP.Controls;
using IEPP.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IEPP.Models
{
    public class Bookmark
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Domain { get; set; }

        public BookmarkContainer ToContainer()
        {
            return new BookmarkContainer() { Title = this.Title, Url = this.Url, Domain=this.Domain };
        }        
    }
}
