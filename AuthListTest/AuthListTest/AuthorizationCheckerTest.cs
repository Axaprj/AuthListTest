using SessionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthListTest
{
    class AuthorizationCheckerTest
    {
        public int ThreadIndex;
        public long UserId;
        public int IterationCount;
        public ISessionManager SessionMgr;

        public void DoTest()
        {
            var session_guid = CreateSession();
            for (int inx = 0; inx < IterationCount; inx++)
            {
                if (SessionMgr.GetSessionUser(session_guid) == null)
                {
                    throw new Exception("Session [" + session_guid + "] not found");
                }
            }
            SessionMgr.CloseSession(session_guid);
        }

        Guid CreateSession()
        {
            return (ThreadIndex % 2 == 0 ?
                SessionMgr.CreateSession(UserId).SessionGuid :
                DbManager.CreateSession(UserId).SessionGuid
            );
        }
    }
}
