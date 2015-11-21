using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace SessionData
{
    public class SessionCache
    {
        const int REFRESH_TIMEOUT_MSEC = 1000;

        readonly ReaderWriterLockSlim Cache_Lock = new ReaderWriterLockSlim();
        readonly Dictionary<Guid, Session> Cache = new Dictionary<Guid, Session>();
        readonly Thread CacheThread;

        public SessionCache()
        {
            foreach (var sess in DbManager.GetSessions())
                Cache.Add(sess.SessionGuid, sess);
            CacheThread = new Thread(new ThreadStart(handlerCacheRefresh));
            CacheThread.Start();
        }

        void handlerCacheRefresh()
        {
            while (true)
            {
                Thread.Sleep(REFRESH_TIMEOUT_MSEC);
                Session[] sessions;
                Cache_Lock.EnterReadLock();
                try
                {
                    sessions = Cache.Values.ToArray();
                }
                finally
                {
                    Cache_Lock.ExitReadLock();
                }
                DbManager.UpdateActivity(sessions);
                sessions = DbManager.GetSessions();
                Cache_Lock.EnterWriteLock();
                try
                {
                    Cache.Clear();
                    foreach (var sess in sessions)
                        Cache.Add(sess.SessionGuid, sess);
                }
                finally
                {
                    Cache_Lock.ExitWriteLock();
                }
            }
        }

        public void AddSession(Session sess)
        {
            Cache_Lock.EnterWriteLock();
            try
            {
                if (Cache.ContainsKey(sess.SessionGuid))
                {
                    Cache.Add(sess.SessionGuid, sess);
                }
            }
            finally
            {
                Cache_Lock.ExitWriteLock();
            }
        }

        public Session GetSession(Guid session_guid)
        {
            Cache_Lock.EnterUpgradeableReadLock();
            try
            {
                if (Cache.ContainsKey(session_guid))
                {
                    return Cache[session_guid];
                }
                else
                {
                    var sess = DbManager.GetSession(session_guid, set_activity: false);
                    if (sess != null)
                    {
                        Cache_Lock.EnterWriteLock();
                        try
                        {
                            Cache.Add(session_guid, sess);
                        }
                        finally
                        {
                            Cache_Lock.ExitWriteLock();
                        }
                    }
                    return sess;
                }
            }
            finally
            {
                Cache_Lock.ExitUpgradeableReadLock();
            }
        }
    }
}
