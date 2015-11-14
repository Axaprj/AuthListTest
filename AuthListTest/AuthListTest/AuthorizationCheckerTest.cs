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
        public long UserId;
        public int IterationCount;
        public ISessionManager SessionMgr;

        public void DoTest()
        {
            SessionMgr.CreateSession(UserId);
            for(int inx=0; inx<IterationCount; inx++)
            {
                SessionMgr.IsSession(UserId);
            }
            SessionMgr.CloseSession(UserId);
        }
    }
}
