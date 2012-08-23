using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace Tavisca.Services.Profiling
{
    public class RestProfilerBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            //do nothing
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            //do nothing
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            var restServerInspector = new RestServerInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(restServerInspector);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            //do nothing
        }

        protected override object CreateBehavior()
        {
            return this;
        }

        public override Type BehaviorType
        {
            get { return this.GetType(); }
        }
    }
}
