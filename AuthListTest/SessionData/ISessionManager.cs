using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionData
{
    interface ISessionManager
    {
        void CreateSession(int user_id);
        bool IsSession(int user_id);
        void CloseSession(int user_id);
    }
}
