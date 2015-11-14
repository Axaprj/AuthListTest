using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionData
{
    public class DbManager
    {
        const int ACTIVITY_GRAIN_MSEC = 3000;

        public static void InitializeDB()
        {
            using (var dbx = new SessionsContainer())
            {
                for (int inx = 0; inx < 1000; inx++)
                {
                    var usr = new User() { Name = "user" + inx };
                    dbx.User.Add(usr);
                }
                dbx.SaveChanges();
            }
        }

        public static void CreateSession(int user_id)
        {
            using (var dbx = new SessionsContainer())
            {
                var ts = DateTime.Now;
                var sess = new Session() { UserId = user_id, Created = ts, LastActivity = ts };
                dbx.Session.Add(sess);
                dbx.SaveChanges();
            }
        }

        public static Session GetSession(int user_id)
        {
            using (var dbx = new SessionsContainer())
            {
                var sess = dbx.Session.Where(s => s.UserId == user_id).FirstOrDefault();
                if (sess != null)
                {
                    var ts = DateTime.Now;
                    if (ACTIVITY_GRAIN_MSEC > ts.Subtract(sess.LastActivity).TotalMilliseconds)
                    {
                        sess.LastActivity = ts;
                        dbx.SaveChanges();
                    }
                }
                return sess;
            }
        }

        public static Session[] GetSessions()
        {
            using (var dbx = new SessionsContainer())
            {
                return dbx.Session.ToArray();
            }
        }
    }
}
