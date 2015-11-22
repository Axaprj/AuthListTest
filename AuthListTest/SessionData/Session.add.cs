using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionData
{
    /// <summary>
    /// Session entity
    /// </summary>
    public partial class Session
    {
        /// <summary>
        /// Required session update activity gain
        /// </summary>
        const int ACTIVITY_GRAIN_MSEC = 1000;
        public bool IsActivityUpdateRequired
        {
            get
            {
                var ts = DateTime.Now;
                return (ACTIVITY_GRAIN_MSEC > ts.Subtract(LastActivity).TotalMilliseconds);
            }
        }

        /// <summary>
        /// Session Guid (string Id parse)
        /// </summary>
        public Guid SessionGuid
        {
            get
            {
                return Guid.Parse(Id);
            }
        }
    }
}
