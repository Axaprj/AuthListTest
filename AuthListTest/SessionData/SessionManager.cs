using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionData
{
    public class SessionManager : ISessionManager
    {
        void ISessionManager.CloseSession(int user_id)
        {
            DbManager.CloseSession(user_id);
        }

        void ISessionManager.CreateSession(int user_id)
        {
            DbManager.CreateSession(user_id);
        }

        bool ISessionManager.IsSession(int user_id)
        {
            return DbManager.GetSession(user_id, set_activity:true) != null;
        }
    }
}
