using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionData
{
    public partial class Session
    {
        const int ACTIVITY_GRAIN_MSEC = 3000;

        public bool IsActivityUpdateRequired
        {
            get
            {
                var ts = DateTime.Now;
                return (ACTIVITY_GRAIN_MSEC > ts.Subtract(LastActivity).TotalMilliseconds);
            }
        }
    }
}
