using SessionData;
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
            Console.WriteLine("Authorization List Checker Test");
            Console.WriteLine("AuthListTest [-RAW] [-ITERATIONS <#>] [-THREADS <#>]");
            bool is_raw = false;
            int thread_cnt = DEFAULT_THREAD_CNT;
            int iter_cnt = DEFAULT_ITERATION_CNT;
            for (int inx = 0; inx < args.Length; inx++)
            {
                if ("-RAW".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    is_raw = true;
                else if ("-ITERATIONS".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    iter_cnt = int.Parse(args[inx + 1]);
                else if ("-THREADS".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    thread_cnt = int.Parse(args[inx + 1]);
            }
            Console.WriteLine("AuthListTest "
                + (is_raw ? "-RAW " : "") + "-ITERATIONS " + iter_cnt + " -THREADS " + thread_cnt);

            Console.WriteLine("DB initialization");
            DbManager.InitializeDB(thread_cnt);
            Console.WriteLine("Done");

            User[] users = DbManager.GetUsers(thread_cnt);
            var started = DateTime.Now;
            for (int i = 0; i < thread_cnt; i++)
            {
                var test = new AuthorizationCheckerTest()
                {
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
            Console.WriteLine("TOTAL: " + (int)DateTime.Now.Subtract(started).TotalMilliseconds + "ms");
        }

        static void TestThreadProc(Object stateInfo)
        {
            var test = (AuthorizationCheckerTest)stateInfo;
            var lbl = "#" + test.ThreadIndex;
            Console.WriteLine(lbl + " start");
            try
            {
                test.DoTest();
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
