using System;

namespace SessionData
{
    public interface ISessionManager
    {
        Session CreateSession(long user_id);
        User GetSessionUser(Guid session_guid);
        void CloseSession(Guid session_guid);
    }
}
