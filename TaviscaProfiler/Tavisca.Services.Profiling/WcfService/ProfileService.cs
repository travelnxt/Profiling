using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tavisca.Services.Profiling.Contract;

namespace Tavisca.Services.Profiling
{
    public class ProfileService : IProfile
    {
        public Debug GetTimings(string transactionId)
        {
            Debug result = null;

            Guid transactionIdGuid;
            if (Guid.TryParse(transactionId, out transactionIdGuid) == false)
                return result;

            var profilerStorage = Activator.CreateInstance(Type.GetType(Configuration.StorageObjectString)) as IProfilerStorage;
            if ((profilerStorage == null) == false)
            {
                result = profilerStorage.GetTimings(transactionId);
            }
            return result;
        }

        public List<SqlTiming> GetSqlTimings(string transactionId, string timingId)
        {
            List<SqlTiming> result = null;

            Guid transactionIdGuid;
            if (Guid.TryParse(transactionId, out transactionIdGuid) == false)
                return result;

            Guid timingIdGuid;
            if (Guid.TryParse(timingId, out timingIdGuid) == false)
                return result;
            
            var profilerStorage = Activator.CreateInstance(Type.GetType(Configuration.StorageObjectString)) as IProfilerStorage;
            if ((profilerStorage == null) == false)
            {
                result = profilerStorage.GetSqlTimings(transactionId, timingId);
            }
            return result;
        }
    }
}
