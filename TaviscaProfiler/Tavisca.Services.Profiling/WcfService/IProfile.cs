using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Tavisca.Services.Profiling.Contract;

namespace Tavisca.Services.Profiling
{
    [ServiceContract(Namespace = "http://www.tavisca.com/profiling/services/2012/3")]
    public interface IProfile
    {
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat= WebMessageFormat.Json,  UriTemplate = "/{transactionId}")]
        Debug GetTimings(string transactionId);

        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/{transactionid}/{timingId}")]
        List<SqlTiming> GetSqlTimings(string transactionid,  string timingId);
    }
}
