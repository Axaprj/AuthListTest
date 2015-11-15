using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SessionData
{
    public class SessionCacheManager : ISessionManager
    {
        class SessionCache
        {
            const int REFRESH_TIMEOUT_MSEC = 1000;

            readonly ReaderWriterLockSlim Cache_Lock = new ReaderWriterLockSlim();
            readonly Dictionary<long, Session> Cache = new Dictionary<long, Session>();
            readonly Thread CacheThread;

            public SessionCache()
            {
                foreach (var sess in DbManager.GetSessions())
                    Cache.Add(sess.UserId, sess);
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
                            Cache.Add(sess.UserId, sess);
                    }
                    finally
                    {
                        Cache_Lock.ExitWriteLock();
                    }
                }
            }

            public void CreateSession(long user_id)
            {
                var sess = DbManager.CreateSession(user_id);
                Cache_Lock.EnterWriteLock();
                try
                {
                    if (Cache.ContainsKey(sess.UserId))
                    {
                        Cache.Add(sess.UserId, sess);
                    }
                }
                finally
                {
                    Cache_Lock.ExitWriteLock();
                }
            }

            public Session GetSession(long user_id)
            {
                Cache_Lock.EnterUpgradeableReadLock();
                try
                {
                    if (Cache.ContainsKey(user_id))
                    {
                        return Cache[user_id];
                    }
                    else
                    {
                        var sess = DbManager.GetSession(user_id, set_activity: false);
                        if (sess != null)
                        {
                            Cache_Lock.EnterWriteLock();
                            try
                            {
                                Cache.Add(user_id, sess);
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

        static readonly object CacheLock = new object();
        static SessionCache _sessCache;
        static SessionCache SessCache
        {
            get
            {
                lock (CacheLock)
                {
                    if (_sessCache == null)
                    {
                        _sessCache = new SessionCache();
                    }
                    return _sessCache;
                }
            }
        }

        void ISessionManager.CloseSession(long user_id)
        {
            DbManager.CloseSession(user_id);
        }

        void ISessionManager.CreateSession(long user_id)
        {
            SessCache.CreateSession(user_id);
        }

        bool ISessionManager.IsSession(long user_id)
        {
            return SessCache.GetSession(user_id) != null;
        }
    }
}
