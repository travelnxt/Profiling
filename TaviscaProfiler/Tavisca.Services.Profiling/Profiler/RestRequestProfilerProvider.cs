using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Profiling;
using System.ServiceModel;
using StackExchange.Profiling.Wcf.Storage;
using System.ServiceModel.Web;
using StackExchange.Profiling.Wcf.Helpers;


namespace Tavisca.Services.Profiling
{
    public class RestRequestProfilerProvider : BaseProfilerProvider
    {
        public RestRequestProfilerProvider()
        {
            MiniProfiler.Settings.Storage =
                MiniProfiler.Settings.Storage ?? new WcfRequestInstanceStorage();
        }

        public override MiniProfiler Start(ProfileLevel level)
        {
            var context = WcfInstanceContext.Current;
            if (context == null) return null;

            var operationContext = OperationContext.Current;
            if (operationContext == null) return null;

            var instanceContext = operationContext.InstanceContext;
            if (instanceContext == null) return null;

            var webOperationContext = WebOperationContext.Current;
            if((webOperationContext == null) == true) return null;
            if (webOperationContext.IncomingRequest.UriTemplateMatch == null) return null;

            

            var result = new MiniProfiler(GetProfilerName(operationContext, instanceContext), level);
            result.Root.AddKeyValue("requesturl",operationContext.IncomingMessageHeaders.To.OriginalString);
            result.Root.AddKeyValue("method", webOperationContext.IncomingRequest.Method);
            
            SetCurrentProfiler(result);
            SetProfilerActive(result);

            return result;
        }

        public override void Stop(bool discardResults)
        {
            var current = GetCurrentProfiler();

            if (current == null)
                return;

            if (!StopProfiler(current))
                return;

            if (discardResults)
            {
                SetCurrentProfiler(null);
                return;
            }

            EnsureServiceName(current);
            SaveProfiler(current);
        }

        public override MiniProfiler GetCurrentProfiler()
        {
            var context = WcfInstanceContext.GetCurrentWithoutInstantiating();
            if (context == null) return null;

            return context.Items[WcfCacheKey] as MiniProfiler;
        }

        private const string WcfCacheKey = "profiler:";

        private void SetCurrentProfiler(MiniProfiler profiler)
        {
            var context = WcfInstanceContext.Current;
            if (context == null) return;

            context.Items[WcfCacheKey] = profiler;
        }

        private static void EnsureServiceName(MiniProfiler profiler)
        {
            if (string.IsNullOrWhiteSpace(profiler.Name))
            {
                var operationContext = OperationContext.Current;
                if (operationContext == null) return;

                var instanceContext = operationContext.InstanceContext;
                if (instanceContext == null) return;


                profiler.Name = GetProfilerName(operationContext, instanceContext);
            }
        }

        private static string GetProfilerName(OperationContext operationContext, InstanceContext instanceContext)
        {
            var action = "Unknown";
            if (string.IsNullOrWhiteSpace(operationContext.IncomingMessageHeaders.To.Query))
                action = operationContext.IncomingMessageHeaders.To.OriginalString;
            else
                action = operationContext.IncomingMessageHeaders.To.OriginalString.Replace(operationContext.IncomingMessageHeaders.To.Query, 
                    string.Empty);

            string serviceName = string.Format("{0} [{1}]",
                instanceContext.Host.Description.Name,
                action);

            return serviceName;
        }
    }
}
