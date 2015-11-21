﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace SessionData
{
    public class DbManager
    {
        public static void InitializeDB(int users_count)
        {
            using (var dbx = new SessionsContainer())
            {
                users_count = users_count - dbx.User.Count();
                for (int inx = 0; inx < users_count; inx++)
                {
                    var usr = new User() { Name = "user" + Guid.NewGuid() };
                    dbx.User.Add(usr);
                }
                foreach (var sess in dbx.Session)
                {
                    dbx.Session.Remove(sess);
                }
                dbx.SaveChanges();
            }
        }

        public static User[] GetUsers(int count)
        {
            using (var dbx = new SessionsContainer())
            {
                return dbx.User.Take(count).ToArray();
            }
        }


        public static void CloseSession(Guid session_guid)
        {
            using (var dbx = new SessionsContainer())
            {
                var sess = dbx.Session.Where(s => s.SessionGuid == session_guid).FirstOrDefault();
                dbx.Session.Remove(sess);
                dbx.SaveChanges();
            }
        }

        public static Session CreateSession(long user_id)
        {
            using (var dbx = new SessionsContainer())
            {
                var ts = DateTime.Now;
                var sess = new Session()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = user_id,
                    Created = ts,
                    LastActivity = ts
                };
                dbx.Session.Add(sess);
                dbx.SaveChanges();
                return sess;
            }
        }

        public static Session GetSession(Guid session_guid, bool set_activity)
        {
            using (var dbx = new SessionsContainer())
            {
                var sqlite_guid = session_guid.ToString();
                var qry =
                    dbx.Session.Include(s => s.User).Where(s => s.Id == sqlite_guid);
                var sess = qry.FirstOrDefault();
                if (set_activity && sess != null)
                {
                    if (sess.IsActivityUpdateRequired)
                    {
                        sess.LastActivity = DateTime.Now;
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
                return dbx.Session.Include(s => s.User).ToArray();
            }
        }

        public static void UpdateActivity(Session[] sessions)
        {
            foreach (var sess in sessions)
            {
                if (sess.IsActivityUpdateRequired)
                {
                    using (var dbx = new SessionsContainer())
                    {
                        var db_sess = dbx.Session.Where(
                            s => s.SessionGuid == sess.SessionGuid).FirstOrDefault();
                        if (db_sess == null) continue;
                        db_sess.LastActivity = DateTime.Now;
                        dbx.SaveChanges();
                    }
                }
            }
        }

    }
}
