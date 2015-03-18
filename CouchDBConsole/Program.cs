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
            CouchDB cDBLib = new CouchDB(AuthenticationSchemes.Basic, "cdb", "cdbpwd!");

            ////Create a db
            Response<string> response = cDBLib.CreateDB("test-1");

            ////get all dbs in the the instance of CouchDB
            //Response<string> response = cDBLib.ListDBs();

            ////fetch information about a db
            //Response<string> response = cDBLib.FetchDB("test-1");

            ////create a document
            //string json = "{\"firstName\":\"John\",\"lastName\":\"Doe\"}";
            //Response<string> response = cDBLib.CreateDocument("test-1", json);

            ////create a document with an id
            //string json = "{\"firstName\":\"John\",\"lastName\":\"Doe\"}"; ;
            //Response<string> response = cDBLib.CreateDocument("test-1", "emp1", json);

            ////fetch a document
            //Response<string> response = cDBLib.FetchDocument("test-1", "emp1");

            ////update a document
            //string json = "{\"firstName\":\"Jane\",\"lastName\":\"Doe\"}";
            //Response<string> response = cDBLib.UpdateDocument("test-1", "emp1", "1-f9584b2364c83ae6e05c670e1c17eeb4", json);

            ////this will update the latest version of the document
            //string json = "{\"firstName\":\"Jane\",\"lastName\":\"Smith\"}";
            //Response<string> response = cDBLib.UpsertDocument("test-1", "emp1", json);

            ////this will create the document
            //string json = "{\"firstName\":\"Jon\",\"lastName\":\"Smith\"}";
            //Response<string> response = cDBLib.UpsertDocument("test-1", "emp2", json);

            ////delete a document
            //Response<string> response = cDBLib.DeleteDocument("test-1", "emp1");

            ////Post bulk doc
            ////{
            ////  "docs": [
            ////    {
            ////      "_id": "emp3",
            ////      "firstName": "John",
            ////      "lastName": "Doe",
            ////      "department": "HR"
            ////    },
            ////    {
            ////      "_id": "emp2",
            ////      "_rev": "1-8cfd1700eceb12adf67c55f96b43b6d0",
            ////      "firstName": "Jon",
            ////      "lastName": "Smith",
            ////      "department": "Accounts"
            ////    },
            ////    {
            ////      "_id": "cc5e82de2f03bf189828b3b390002161",
            ////      "_rev": "1-f9584b2364c83ae6e05c670e1c17eeb4",
            ////      "_deleted": true
            ////    }
            ////  ]
            ////}
            ////The first document will be created, the second document will be updated with the department information, the third document will be deleted.
            //string bulkdoc = "{\"docs\":[{\"_id\":\"emp3\",\"firstName\":\"John\",\"lastName\":\"Doe\",\"department\":\"HR\"},{\"_id\":\"emp2\",\"_rev\":\"1-8cfd1700eceb12adf67c55f96b43b6d0\",\"firstName\":\"Jon\",\"lastName\":\"Smith\",\"department\":\"Accounts\"},{\"_id\":\"cc5e82de2f03bf189828b3b390002161\",\"_rev\":\"1-f9584b2364c83ae6e05c670e1c17eeb4\",\"_deleted\":true}]}";
            //Response<string> response = cDBLib.PostBulkDocument("test-1", bulkdoc);

            ////query data
            ////condiser a design doc in the db test-1 with the view 'employees':
            ////{
            ////  "_id": "_design/employee",
            ////  "_rev": "1-f14e4609b5612178594dac4b1efa3984",
            ////  "views": {
            ////    "employees": {
            ////      "map": "function(doc) {
            ////                if(doc.department){
            ////                  emit(doc.department, null);
            ////                }
            ////              }",
            ////      "reduce": "_count"
            ////    }
            ////  },
            ////  "language": "javascript"
            ////}
            ////This allows us to query the data in many ways.

            ////1. get all the employees 
            //string query = @"/_design/employee/_view/employees?include_docs=true&reduce=false";
            //Response<string> response = cDBLib.FetchDocuments("test-1", query);

            ////2. get all the employees in a department, say HR
            //string query = @"/_design/employee/_view/employees?key=""HR""&include_docs=true&reduce=false";
            //Response<string> response = cDBLib.FetchDocuments("test-1", query);

            ////3. get the total count of employees
            //string query = @"/_design/employee/_view/employees";
            //Response<string> response = cDBLib.FetchDocuments("test-1", query);

            ////4. get the total count of employees in a department, say 'Accounts'
            //string query = @"/_design/employee/_view/employees?key=""Accounts""";
            //Response<string> response = cDBLib.FetchDocuments("test-1", query);

            ////delete the database
            //Response<string> response = cDBLib.DeleteDB("test-1");

            Console.WriteLine("Success: {0}, StatusCode: {1}, ReasonPhrase: {2}", response.Success, response.StatusCode, response.ReasonPhrase);
            Console.WriteLine("Message: {0}", response.Message);
            Console.WriteLine("Content: " + response.Content);

            Console.ReadLine();
        }
    }
}
