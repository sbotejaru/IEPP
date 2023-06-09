using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEPP.ViewModels
{
    public class SettingsVM
    {
        private MainVM mainWinDC;

        public MainVM MainWinDC
        {
            get { return mainWinDC; }
            set { mainWinDC = value; }
        }

        public SettingsVM()
        {

        }
    }
}
