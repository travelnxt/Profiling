using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Books.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            BookReference.BooksClient bc = new BookReference.BooksClient();
            var result = bc.GetBooks();
            bc.Close();
        }
    }
}
