using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using StackExchange.Profiling.Wcf;
using StackExchange.Profiling;

namespace Tavisca.Services.Profiling
{
    public class SoapServerInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            IProfiler profiler = Profiler.Instance;
            profiler.Start();
            var transactionId = Guid.NewGuid().ToString();
            ProfilerRequestHeader profilerReqHeader = null;

            var headerIndex = request.Headers.FindHeader(ProfilerRequestHeader.HeaderName, ProfilerRequestHeader.HeaderNamespace);
            if (headerIndex >= 0)
            {
                profilerReqHeader = request.Headers.GetHeader<ProfilerRequestHeader>(headerIndex);
                if (profilerReqHeader != null) {
                    if (!string.IsNullOrWhiteSpace(profilerReqHeader.TransactionId)) {
                        transactionId = profilerReqHeader.TransactionId;
                        profiler.User = profilerReqHeader.User ?? "Anonymous";
                        profiler.Enabled = profilerReqHeader.DebugEnabled;
                    }
                }
            }
            profiler.TransactionId = transactionId;
            return profilerReqHeader;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            IProfiler profiler = Profiler.Instance;
            bool fault = reply.IsFault;
            if (fault)
                profiler.AddMetaData("code", "fault");
            else
                profiler.AddMetaData("code", "success");
            profiler.Stop();
        }
    }
}
