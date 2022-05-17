using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverlightGIS.Common
{
    public class TrackInfo
    {
        public string ID
        {
            get;
            set;
        }
        public string UserName
        {
            get;
            set;
        }

        public DateTime TrackTime
        {
            get;
            set;
        }

        public double POSX
        {
            get;
            set;
        }

        public double POSY
        {
            get;
            set;
        }
    }
}
