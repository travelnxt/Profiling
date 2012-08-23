using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using StackExchange.Profiling;

namespace Tavisca.Services.Profiling.Threads
{
    public class ThreadFactory
    {
        public static Thread CreateNew(ParameterizedThreadStart paramThreadStart)
        {
            var operationContext = OperationContext.Current;
            var parentThreadId = Thread.CurrentThread.ManagedThreadId;

            return new Thread(arg =>
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;

                using (OperationContextScope scope = new OperationContextScope(operationContext))
                {
                    MiniProfiler.Current.SetHeadForChildThread(parentThreadId, currentThreadId);
                    System.Diagnostics.Debug.WriteLine("Parent:" + parentThreadId + " Current: " + currentThreadId);

                    paramThreadStart(arg);
                }
            });
        }

        public static Thread CreateNew(ThreadStart threadStart)
        {
            var operationContext = OperationContext.Current;
            var parentThreadId = Thread.CurrentThread.ManagedThreadId;

            return new Thread(() =>
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                
                using (OperationContextScope scope = new OperationContextScope(operationContext))
                {
                    MiniProfiler.Current.SetHeadForChildThread(parentThreadId, currentThreadId);
                    System.Diagnostics.Debug.WriteLine("Parent:" + parentThreadId + " Current: " + currentThreadId);
                    threadStart();
                }
            });
        }
    }
}
