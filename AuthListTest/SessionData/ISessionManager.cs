using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionData
{
    public interface ISessionManager
    {
        void CreateSession(long user_id);
        bool IsSession(long user_id);
        void CloseSession(long user_id);
    }
}
