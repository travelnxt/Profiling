using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tavisca.Services.Profiling;
using System.Threading;


namespace DesktopAppSample
{
    class Program
    {
        static void Main(string[] args)
        {
            new TestApp().Run();
            Console.ReadLine();
        }
    }

    public class TestApp
    {
        public void Run()
        {
            //get the singleton profiler
            var profiler = Profiler.Instance;
            
            /*
             * start the profiler (one time operation)
             * explicitly mention the environment and name of the profiler
             * if the environment is other than WCF
             */
            profiler.Start(ProfilerEnvironment.Desktop, "myapp");
            
            //do other operation
            Internal1();

            // 1 time operation
            // stop the profiler and save the profiler info in db
            // can pass optional info to save it or not or by async
            profiler.Stop();
        }

        private void Internal1()
        {
            // get the current instance
            var profiler = Profiler.Instance;
            // defined the scope of profiling
            using (profiler.Step("First"))
            {
                Thread.Sleep(200);
                Internal2();
            }
        }

        private void Internal2()
        {
            // get the current instance
            var profiler = Profiler.Instance;
            // defined the scope of profiling
            // this profiling scope has a parent profiling scope
            // but it will work as independent scope
            using (profiler.Step("Second"))
            {
                Thread.Sleep(500);
            }
        }
    }
}
