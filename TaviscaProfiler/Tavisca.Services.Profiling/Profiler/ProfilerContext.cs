using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Tavisca.Services.Profiling
{
    public class ProfilerContext<T> : IExtension<InstanceContext>
    {
        private T _context;

        public T Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public static ProfilerContext<T> Current
        {
            get
            {
                if (OperationContext.Current == null) return null;
                var context = OperationContext.Current.InstanceContext.Extensions.Find<ProfilerContext<T>>();
                if ((context == null) == true)
                {
                    context = new ProfilerContext<T>();
                    OperationContext.Current.InstanceContext.Extensions.Add(context);
                }
                return context;
            }
        }

        public void Attach(InstanceContext owner) { }

        public void Detach(InstanceContext owner) { }
    }
}
