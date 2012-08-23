using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace Tavisca.Services.Profiling
{
    public class SoapProfilerBehavior : BehaviorExtensionElement, IEndpointBehavior
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
            var soapServerInspector = new SoapServerInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(soapServerInspector);
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
