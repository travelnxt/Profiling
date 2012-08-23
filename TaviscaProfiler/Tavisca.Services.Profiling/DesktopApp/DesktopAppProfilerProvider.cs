using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Profiling;
using StackExchange.Profiling.Storage;
using StackExchange.Profiling.Wcf;

namespace Tavisca.Services.Profiling
{
    public class DesktopAppProfilerProvider : BaseProfilerProvider
    {
        public string Name { get; private set; }

        public DesktopAppProfilerProvider(string name)
        {
            MiniProfiler.Settings.Storage = MiniProfiler.Settings.Storage ?? new DesktopAppInstanceStorage();
            Name = name ?? "Unknown";
        }

        public override MiniProfiler Start(ProfileLevel level)
        {
            var profiler = new MiniProfiler(Name, level);
            new MiniProfilerIntanceContext(profiler);
            profiler.User = new EmptyUserProvider().GetUser();
            SetProfilerActive(profiler);
            return profiler;
        }

        public override void Stop(bool discardResults)
        {
            var current = GetCurrentProfiler();
            if ((current == null) == true) return;

            if (!StopProfiler(current))
                return;

            if (discardResults) {
                var profiler = MiniProfilerIntanceContext.Current;
                profiler = null;
                return;
            }

            SaveProfiler(current);
        }

        public override MiniProfiler GetCurrentProfiler()
        {
            return MiniProfilerIntanceContext.Current;
        }
    }

    public class DesktopAppInstanceStorage : IStorage
    {
        public void Save(MiniProfiler profiler)
        {
            if ((profiler == null) == true)
                return;
            new MiniProfilerIntanceContext(profiler);
        }

        public MiniProfiler Load(Guid id)
        {
            return MiniProfilerIntanceContext.Current;
        }

        public void SetUnviewed(string user, Guid id)
        {
            /*do nothing*/
        }

        public void SetViewed(string user, Guid id)
        {
            /*do nothing*/
        }

        public List<Guid> GetUnviewedIds(string user)
        {
            return new List<Guid>();
        }

        public IEnumerable<Guid> List(int maxResults, DateTime? start = null, DateTime? finish = null, ListResultsOrder orderBy = ListResultsOrder.Decending)
        {
            return new List<Guid>();
        }
    }

    public abstract class ApplicationContextScope<TContext> : IDisposable
    {
        protected ApplicationContextScope()
            : this(ApplicationContextScope<TContext>.Current)
        { }

        protected ApplicationContextScope(TContext scopeObject)
        {
            if (_stack == null)
            {
                _stack = new Stack<TContext>();
            }
            _stack.Push(scopeObject);

        }

        [ThreadStatic]
        private static Stack<TContext> _stack;
        private bool _isDisposed = false;
        protected abstract void Complete(TContext currentContext);

        public void Dispose()
        {
            if (_isDisposed) return;
            Complete(ApplicationContextScope<TContext>.Current);
            _stack.Pop();
            if (_stack.Count == 0)
                _stack = null;
            _isDisposed = true;
        }

        public static TContext Current
        {
            get
            {
                if (_stack == null || _stack.Count == 0)
                    return default(TContext);
                return _stack.Peek();
            }
        }
    }

    public class MiniProfilerIntanceContext : ApplicationContextScope<MiniProfiler>
    {
        public MiniProfilerIntanceContext(MiniProfiler profiler) : base(profiler)
        { }

        protected override void Complete(MiniProfiler currentContext)
        {
            /*do nothing*/
        }
    }
}
