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
            Console.WriteLine("AuthListTest [-INIT] [-RAW] [-ITERATIONS <#>] [-THREADS <#>]");
            bool is_raw = false;
            bool is_init = false;
            int thread_cnt = DEFAULT_THREAD_CNT;
            int iter_cnt = DEFAULT_ITERATION_CNT;
            for (int inx = 0; inx < args.Length; inx++)
            {
                if ("-RAW".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    is_raw = true;
                else if ("-INIT".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    is_init = true;
                else if ("-ITERATIONS".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    iter_cnt = int.Parse(args[inx + 1]);
                else if ("-THREADS".Equals(args[inx], StringComparison.InvariantCultureIgnoreCase))
                    thread_cnt = int.Parse(args[inx + 1]);
            }
            Console.WriteLine("AuthListTest "
                + (is_init ? "-INIT " : "") + (is_raw ? "-RAW " : "") 
                + " -ITERATIONS " + iter_cnt + " -THREADS " + thread_cnt);

            if (is_init)
            {
                Console.WriteLine("DB initialization");
                DbManager.InitializeDB(thread_cnt);
                Console.WriteLine("Done");
            }

            User[] users = DbManager.GetUsers(thread_cnt);
            var started = DateTime.Now;
            for (int i = 0; i < thread_cnt; i++)
            {
                var tinfo = new
                {
                    UserId = users[i].Id,
                    IsRaw = is_raw,
                    IterationCount = iter_cnt,
                    ThreadIndex = i
                };
                ThreadPool.QueueUserWorkItem(new WaitCallback(TestThreadProc), tinfo);
            }
            while (Interlocked.Read(ref FinishedThreadsCount) < thread_cnt)
            {
                Thread.Sleep(1);
            }
            Console.WriteLine("TOTAL: " + (int)DateTime.Now.Subtract(started).TotalMilliseconds + "ms");
        }

        static void TestThreadProc(Object stateInfo)
        {
            var tinfo = (dynamic)stateInfo;
            var lbl = "#" + tinfo.ThreadIndex;
            Console.WriteLine(lbl + " start");
            try
            {
                var test = new AuthorizationCheckerTest() {
                    UserId = tinfo.UserId,
                    IterationCount = tinfo.IterationCount,
                    SessionMgr = (tinfo.IsRaw ? new SessionManager() : null)
                };
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
