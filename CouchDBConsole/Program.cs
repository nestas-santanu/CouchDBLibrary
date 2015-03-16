using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CouchDBLibrary;
using System.Net;

namespace CouchDBConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CouchDB cDBLib = new CouchDB(AuthenticationSchemes.Basic, "cadmin", "cadminpwd");
            //Response<string> response = cDBLib.CreateDB("test-1");

            Response<string> response = cDBLib.ListDBs();
            Console.WriteLine ("{0}, {1}", response.StatusCode, response.ReasonPhrase);
            Console.WriteLine("Message: {0}", response.Message);
            Console.WriteLine("Content: " + response.Content);

            Console.ReadLine();
        }
    }
}
