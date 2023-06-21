using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IEPP.Controls
{
    /// <summary>
    /// BookmarkContainer with a delete button. Used in Settings tab
    /// </summary>
    public class BookmarkSettingsContainer : ContentControl
    {
        public BookmarkContainer Bookmark { get; set; }

        public BookmarkSettingsContainer(BookmarkContainer bookmark)
        {
            Bookmark = bookmark;
        }
    }
}
