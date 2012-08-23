using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Books.Server
{
    [ServiceContract]
    public interface IBooks
    {
        [OperationContract]
        List<Book> GetBooks();

        [OperationContract]
        List<Book> GetBooksMultiThread();
    }

    [DataContract]
    public class Book
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Auther { get; set; }

        [DataMember]
        public int Pages { get; set; }
    }
}
