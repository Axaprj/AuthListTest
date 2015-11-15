using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionData
{
    public class SessionCacheManager : ISessionManager
    {
        void ISessionManager.CloseSession(long user_id)
        {
            DbManager.CloseSession(user_id);
        }

        void ISessionManager.CreateSession(long user_id)
        {
            DbManager.CreateSession(user_id);
        }

        bool ISessionManager.IsSession(long user_id)
        {
            return DbManager.GetSession(user_id, set_activity:true) != null;
        }
    }
}
