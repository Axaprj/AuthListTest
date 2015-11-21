using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace SessionData
{
    public class SessionCache : IDisposable
    {
        const int REFRESH_TIMEOUT_MSEC = 1000;

        readonly ReaderWriterLockSlim Cache_Lock = new ReaderWriterLockSlim();
        readonly Dictionary<Guid, Session> Cache = new Dictionary<Guid, Session>();
        readonly Thread CacheThread;
        DateTime RefreshTS = DateTime.MinValue;

        public SessionCache()
        {
            CacheThread = new Thread(new ThreadStart(handlerCacheRefresh));
            CacheThread.Start();
        }

        void handlerCacheRefresh()
        {
            while (true)
            {
                var obsolescence_msec = DateTime.Now.Subtract(RefreshTS).TotalMilliseconds;
                if(obsolescence_msec < REFRESH_TIMEOUT_MSEC)
                {
                    Thread.Sleep((int)(REFRESH_TIMEOUT_MSEC - obsolescence_msec));
                }

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
                RefreshTS = DateTime.Now;
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
                if (!Cache.ContainsKey(sess.SessionGuid))
                {
                    Cache.Add(sess.SessionGuid, sess);
                }
            }
            finally
            {
                Cache_Lock.ExitWriteLock();
            }
        }

        public void RemoveSession(Guid session_guid)
        {
            Cache_Lock.EnterWriteLock();
            try
            {
                if (Cache.ContainsKey(session_guid))
                {
                    Cache.Remove(session_guid);
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

        #region IDisposable Support
        private bool disposedValue = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CacheThread.Abort();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
