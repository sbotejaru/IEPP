using IEPP.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEPP.Utils
{
    public class Bookmark
    {
        public string Title { get; set; }
        public string Url { get; set; }
        // favicon?
        public BookmarkContainer ToContainer()
        {
            return new BookmarkContainer() { Title = this.Title, Url = this.Url };
        }
    }
}
