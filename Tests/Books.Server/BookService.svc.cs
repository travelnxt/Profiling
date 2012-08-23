using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Tavisca.Services.Profiling;
using System.Threading;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using Tavisca.Services.Profiling.Threads;
using StackExchange.Profiling;

namespace Books.Server
{
    public class BooksService : IBooks
    {
        public List<Book> GetBooksMultiThread()
        {
            System.Diagnostics.Debug.WriteLine("GetBooksMultiThread - " + Thread.CurrentThread.ManagedThreadId.ToString());
            List<Book> result = null;
            
            IProfiler profiler = Profiler.Instance;
            using (profiler.Step("GetBooksMultiT"))
            {
                result = InternalGetMultiTBooks();
            }
            return result;
        }

        private List<Book> InternalGetMultiTBooks()
        {
            List<Book> result = new List<Book>();
            IProfiler profiler = Profiler.Instance;

            using (profiler.Step("InternalGetMultiT1"))
            {
                Object sync = new object();
                var t1 =  MTPrivate1(result, sync);
                var t2 = MTPrivate2(result, sync);
                t1.Start();
                t2.Start();
                t1.Join();
                t2.Join();

                using (profiler.Step("MT3"))
                {
                    for (int i = 0; i < 1000; i++)
                    { }
                }
            }

            using (profiler.Step("internalgetmultitouter"))
            {
                for (int i = 0; i < 1000; i++)
                { }
            }
            
            return result;
        }

        private Thread MTPrivate2(List<Book> result, Object sync)
        {
            Thread t2 = ThreadFactory.CreateNew(new ThreadStart(() =>
            {
                IProfiler profiler = Profiler.Instance;
                using (profiler.Step("MT2"))
                {
                    lock (sync)
                    {
                        result.Add(new Book { Auther = "T2Author", Name = "T2Name", Pages = 22 });
                        SqlTestMethod();
                        //Thread.Sleep(5000);
                    }
                }
            }));
            System.Diagnostics.Debug.WriteLine("MT2 - " + t2.ManagedThreadId.ToString());
            t2.Name = "mt2";

                return t2;
        }

        private  Thread MTPrivate1(List<Book> result, Object sync)
        {
            Thread t1 =  ThreadFactory.CreateNew(new ThreadStart(() =>
            {
                IProfiler profiler = Profiler.Instance;
                using (profiler.Step("MT1"))
                {
                    lock (sync)
                    {
                        result.Add(new Book { Auther = "T1Author", Name = "T1Name", Pages = 2 });
                        LinqTestMethod();
                        //Thread.Sleep(5000);
                    }
                }
            }));

            System.Diagnostics.Debug.WriteLine("MT1 - " + t1.ManagedThreadId.ToString());
            t1.Name = "Mt1";

                return t1;
        }

        public List<Book> GetBooks()
        {
            List<Book> result = null;

            var profiler = Profiler.Instance;
            using (profiler.Step("getbooks"))
            {
                Internal1();
                using (profiler.Step("GeneratingResult"))
                {
                    result = new List<Book>() { new Book() { Auther = "Abc", Name = "book1", Pages = 5}, 
                        new Book() { Auther = "Xyz", Name = "book2", Pages = 50}};
                    SqlTestMethod();
                }
            }
            return result;
        }

        private void Internal1()
        {
            var profiler = Profiler.Instance;
            using (profiler.Step("Internal.Step1"))
            {
                //Thread.Sleep(500);
                Internal2();
            }
        }

        private void Internal2()
        {
            var profiler = Profiler.Instance;
            using (profiler.Step("Internal.Step2"))
            {
                //Thread.Sleep(500);
                LinqTestMethod();
            }
        }

        private string LinqTestMethod()
        {
            var profiler = Profiler.Instance;
            using (profiler.Step("LinqTestMethod"))
            {
                PrivateLinqTestMethod();
            }
            return "done";
        }

        private static void PrivateLinqTestMethod()
        {
            var context = GetLinqContext();
            var profiler = Profiler.Instance;
            using (profiler.Step("LinqGetPerson"))
            {
                var result = context.GetPerson(1).SingleOrDefault();
                if ((result == null) == false)
                {
                    var name = result.Name;
                }
            }
            context.Dispose();
        }

        private static TestDbDataContext GetLinqContext()
        {
            var conn = GetSqlServerConnection();
            var context = new TestDbDataContext(conn);
            return context;
        }

        private string SqlTestMethod()
        {
            var profiler = Profiler.Instance;
            using (profiler.Step("SqlTestMethod"))
            {
                PrivateSqlTestMethod();
            }
            return "done";
        }

        private static void PrivateSqlTestMethod()
        {
            using (var conn = GetSqlServerConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "select * from person";
                cmd.Connection = conn;


                var profiler = Profiler.Instance;
                using (profiler.Step("SqlGetPerson"))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string value = reader[2].ToString();
                        }
                    }
                }
            }
        }

        private static DbConnection GetSqlServerConnection()
        {
            DbConnection conn = null;
            using (Profiler.Instance.Step("GettingSqlServerConnection"))
            {
                conn = new SqlConnection(ConfigurationManager.ConnectionStrings["testprofilierdbConnectionString"].ConnectionString);
                if ((Profiler.Instance == null) == false)
                    conn = new ProfilerDbConnection(conn);
                return conn;
            }
        }
    }
}
