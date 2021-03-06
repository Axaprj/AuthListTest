﻿using SessionData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AuthListTest
{
    class Program
    {
        const int DEFAULT_THREAD_CNT = 20;
        const int DEFAULT_ITERATION_CNT = 100;
        static long FinishedThreadsCount = 0;

        static void Main(string[] args)
        {
            bool is_raw = false;
            bool is_silent = false;
            int thread_cnt = DEFAULT_THREAD_CNT;
            int iter_cnt = DEFAULT_ITERATION_CNT;
            for (int inx = 0; inx < args.Length; inx++)
            {
                if ("-RAW".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    is_raw = true;
                if ("-SILENT".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    is_silent = true;
                else if ("-ITERATIONS".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    iter_cnt = int.Parse(args[inx + 1]);
                else if ("-THREADS".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    thread_cnt = int.Parse(args[inx + 1]);
            }
            if (!is_silent)
            {
                Console.WriteLine("Authorization List Checker Test");
                Console.WriteLine("AuthListTest [-RAW] [-SILENT] [-ITERATIONS <#>] [-THREADS <#>]");
                Console.WriteLine("DB initialization");
            }
            DbManager.InitializeDB(thread_cnt);
            if (!is_silent)
                Console.WriteLine("Done");

            User[] users = DbManager.GetUsers(thread_cnt);
            var started = DateTime.Now;
            ThreadPool.SetMaxThreads(thread_cnt, thread_cnt);
            ThreadPool.SetMinThreads(thread_cnt, thread_cnt);
            for (int i = 0; i < thread_cnt; i++)
            {
                var test = new AuthorizationCheckerTest()
                {
                    IsSilent = is_silent, 
                    ThreadIndex = i,
                    UserId = users[i].Id,
                    IterationCount = iter_cnt,
                    SessionMgr = (is_raw ?
                        (ISessionManager)new SessionManager() : new SessionCacheManager())
                };

                ThreadPool.QueueUserWorkItem(new WaitCallback(TestThreadProc), test);
            }
            while (Interlocked.Read(ref FinishedThreadsCount) < thread_cnt)
            {
                Thread.Sleep(0);
            }
            var total = (int)DateTime.Now.Subtract(started).TotalMilliseconds;
            if (is_silent)
                Console.WriteLine(total);
            else
                Console.WriteLine("TOTAL(" +
                (is_raw ? "RAW" : "CACHE") + ", ITERATIONS:" + iter_cnt + ", THREADS:" + thread_cnt + ") = "
                + total + "ms");
            SessionCacheManager.TerminateCaching();
        }

        static void TestThreadProc(Object stateInfo)
        {
            var test = (AuthorizationCheckerTest)stateInfo;
            var lbl = "#" + test.ThreadIndex;
            if(!test.IsSilent)
                Console.WriteLine(lbl + " start");
            try
            {
                test.DoTest();
                if (!test.IsSilent)
                    Console.WriteLine(lbl + " stop");
            }
            catch (Exception ex)
            {
                Console.WriteLine(lbl + "\n" + ex.ToString());
            }
            Interlocked.Increment(ref FinishedThreadsCount);
        }



    }
}
