# CouchDBLibrary
**A C# library for CouchDB**

This library abstracts the HTTP requests to a CouchDB.
It is a work in progress, but the methods available should be sufficient for CRUD operations on a CouchDB database.
Tested on CouchDB version 1.6.

Download the CouchDBLibrary to include in a .NET solution or project.

The library consists of 3 class files:

1. `CouchDB.cs`: Contains the methods for communicating with a CouchDB database
2. `Response.cs`: All methods return a Response object
3. `Authentication.cs`: The library currently supports only the Basic Authentication Scheme.

###Response.cs

```csharp
namespace CouchDBLibrary
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public string Message { get; set; }
        public T Content { get; set; }
    }
}
```

The `Content` property is capable of returning any primitive type or object; however, all methods of `CouchDB.cs` return a `Response` object of type `Response<string>`. The value of the `Content` property, if available, is always a string in JSON format, as returned by CouchDB. In case of an Exception, it may not have a value.

For example, a request to list all DB's on a local CouchDB instance, will return the following response, with the `Content` property containing the databases in the CouchDB instance.

```csharp
Success: True 
StatusCode: 200 
ReasonPhrase: OK
Message: The Content property contains the list of databases in the CouchDB at http://127.0.0.1:5984.
Content: [
  "_replicator",
  "_users",
  "test-1",
  "test-2",
  "wvs-test_sample_2-xlsx"
]
```

###CouchDb.cs

####Constructor:

```csharp
public CouchDB( AuthenticationSchemes authenticationScheme, string userName, string password, string baseUrl = @"http://127.0.0.1:5984"){..}
```

+ `"authenticationScheme"`: A scheme defined in the Enum `System.Net.AuthenticationSchemes`. Only the Basic scheme is supported in this library at the moment.
+ `"userName"/ "password"`: Credentials to query the CouchDB.
+ `"baseUrl"`: Defines the instance of the CouchDB to query. Default is `http://127.0.0.1:5984`

######Usage:
```csharp
CouchDB cDBLib = new CouchDB(AuthenticationSchemes.Basic, "cadmin", "cadminpwd");
```
****

####Methods:
+ [CreateDB](#createdb)
+ [ListDBs](#listdbs)
+ [FetchDB](#fetchdb)
+ [CompactDatabase](#compactdatabase)
+ [CompactView](#compactview)
+ [DeleteDB](#deletedb)
+ [CreateDocument](#createdocument)
+ [CreateDocument](#createdocument---overloaded)
+ [FetchDocument](#fetchdocument)
+ [UpdateDocument](#updatedocument)
+ [UpsertDocument](#upsertdocument)
+ [DeleteDocument](#deletedocument)
+ [PostBulkDocument](#postbulkdocument)
+ [FetchDocuments](#fetchdocuments)

****

#####CreateDB
Creates a database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> CreateDB(string dbName){..}
```
+ `"dbName"`: The name of the database to be created.

######Usage: 
```csharp
Response<string> response = cDBLib.CreateDB("test-1");
```

The response is
```csharp
Success: True, StatusCode: 201, ReasonPhrase: Created
Message: The database was created at http://127.0.0.1:5984/test-1.
Content: {
  "ok": true
}
```
****

#####ListDBs
Returns a list of DBs in the queried instance of CouchDB.
```csharp
public Response<string> ListDBs(){..}
```
######Usage: 
```csharp
Response<string> response = cDBLib.ListDBs();
```

The response is
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message: The Content property contains the list of databases in the CouchDB at http://127.0.0.1:5984.
Content: [
  "_replicator",
  "_users",
  "test-1",
  "wvs-test_sample_2-xlsx"
]
```
****

#####FetchDB
Checks if the database exists in the queried instance of CouchDB. Returns its information, if it exists.
```csharp
public Response<string> FetchDB(string dbName){..}
```
+ `"dbName"`: The name of the database.

######Usage: 
```csharp
Response<string> response = cDBLib.FetchDB("test-1");
```

The response is
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message: The database exists. The Content property contains the information of the database test-1.
Content: {
  "db_name": "test-1",
  "doc_count": 0,
  "doc_del_count": 0,
  "update_seq": 0,
  "purge_seq": 0,
  "compact_running": false,
  "disk_size": 79,
  "data_size": 0,
  "instance_start_time": "1426663741232830",
  "disk_format_version": 6,
  "committed_update_seq": 0
}
```
****

#####CompactDatabase
Compacts the database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> CompactDatabase(string dbName){..}
```
+ `"dbName"`: The name of the database to compact.

######Usage: 
```csharp
Response<string> response = cDBLib.CompactDatabase("test-1");
```
****

#####CompactView
Compacts the views in the design document 'designDoc' in the database 'dbName'.
```csharp
public Response<string> CompactView(string dbName, string designDoc){..}
```
+ `"dbName"`: The name of the database to compact.
+ `"designDoc"`: The name of the design document.

######Usage: 
```csharp
Response<string> response = cDBLib.CompactView("test-1", "test-1_ddoc");
```
****

#####DeleteDB
Deletes the database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> DeleteDB(string dbName){..}
```
+ `"dbName"`: The name of the database to delete.

######Usage: 
```csharp
Response<string> response = cDBLib.DeleteDB("test-1");
```
****

#####CreateDocument
Creates a document in the database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> CreateDocument(string dbName, string document){..}
```
+ `"dbName"`: The name of the database.
+ `"document"`: The document to be created in the database. Must be a JSON string.

######Usage: 
```csharp
string json = "{\"firstName\":\"John\",\"lastName\":\"Doe\"}";
Response<string> response = cDBLib.CreateDocument("test-1", json);
```

The id of the document is created by CouchDB.
```csharp
Success: True, StatusCode: 201, ReasonPhrase: Created
Message: The document was created.
Content: {
  "ok": true,
  "id": "cc5e82de2f03bf189828b3b390002161",
  "rev": "1-f9584b2364c83ae6e05c670e1c17eeb4"
}
```
****

#####CreateDocument - overloaded
Creates a document in the database 'dbName' with the id 'documentId'in the queried instance of CouchDB.
```csharp
public Response<string> CreateDocument(string dbName, string documentId, string document){..}
```
+ `"dbName"`: The name of the database.
+ `"documentId"`: The id of the document to be created.
+ `"document"`: The document to be created in the database. Must be a JSON string.

######Usage: 
```csharp
string json = "{\"firstName\":\"John\",\"lastName\":\"Doe\"}";
Response<string> response = cDBLib.CreateDocument("test-1", "emp1", json);
```

The response is:
```csharp
Success: True, StatusCode: 201, ReasonPhrase: Created
Message: The document with documentId = emp1 was created.
Content: {
  "ok": true,
  "id": "emp1",
  "rev": "1-f9584b2364c83ae6e05c670e1c17eeb4"
}
```
****

#####FetchDocument
Fetches the document with id 'documentId' from the database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> FetchDocument(string dbName, string documentId){..}
```
+ `"dbName"`: The name of the database.
+ `"documentId"`: The id of the document to be fetched.

######Usage: 
```csharp
Response<string> response = cDBLib.FetchDocument("test-1", "emp1");
```

The response is:
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message:
Content: {
  "_id": "emp1",
  "_rev": "1-f9584b2364c83ae6e05c670e1c17eeb4",
  "firstName": "John",
  "lastName": "Doe"
}
```
****

#####UpdateDocument
Updates a document with 'documentId' in the database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> UpdateDocument(string dbName, string documentId, string revisionId, string document){..}
```
+ `"dbName"`: The name of the database.
+ `"documentId"`: The id of the document to be updated.
+ `"revisionId"`: The revision of the document to be updated.
+ `"document"`: The document to be updated in the database. Must be a JSON string.

######Usage: 
```csharp
string json = "{\"firstName\":\"Jane\",\"lastName\":\"Doe\"}";
Response<string> response = cDBLib.UpdateDocument("test-1", "emp1", "1-f9584b2364c83ae6e05c670e1c17eeb4", json);
```

The response is:
```csharp
Success: True, StatusCode: 201, ReasonPhrase: Created
Message: The document with documentId = emp1 was updated.
Content: {
  "ok": true,
  "id": "emp1",
  "rev": "2-8f867724dcd547ad31571d61412d844a"
}
```
And a `FetchDocument` indicates that the data has been updated.
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message:
Content: {
  "_id": "emp1",
  "_rev": "2-8f867724dcd547ad31571d61412d844a",
  "firstName": "Jane",
  "lastName": "Doe"
}
```
****

#####UpsertDocument
Checks for a document with id 'documentId' in the database 'dbName' in the queried instance of CouchDB. 
If not found, creates the document; if found, updates the latest revision of the document.
```csharp
public Response<string> UpsertDocument(string dbName, string documentId, string document){..}
```
+ `"dbName"`: The name of the database.
+ `"documentId"`: The id of the document.
+ `"document"`: The document to be created/updated in the database. Must be a JSON string.

######Usage: 
```csharp
//this will update the latest version of the document (changing lastNme from Doe to Smith
string json = "{\"firstName\":\"Jane\",\"lastName\":\"Smith\"}";
Response<string> response = cDBLib.UpsertDocument("test-1", "emp1", json);
```

The response is:
```csharp
Success: True, StatusCode: 201, ReasonPhrase: Created
Message: The document with documentId = emp1 was updated.
Content: {
  "ok": true,
  "id": "emp1",
  "rev": "3-20c72eeb4e24c84c967a88f4ed7d2569"
}
```
And a `FetchDocument` indicates that the document has been updated.
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message:
Content: {
  "_id": "emp1",
  "_rev": "3-20c72eeb4e24c84c967a88f4ed7d2569",
  "firstName": "Jane",
  "lastName": "Smith"
}
```

######Usage: 
```csharp
//this will create the document
string json = "{\"firstName\":\"Jon\",\"lastName\":\"Smith\"}";
Response<string> response = cDBLib.UpsertDocument("test-1", "emp2", json);
```

The response is:
```csharp
Success: True, StatusCode: 201, ReasonPhrase: Created
Message: The document with documentId = emp2 was created.
Content: {
  "ok": true,
  "id": "emp2",
  "rev": "1-8cfd1700eceb12adf67c55f96b43b6d0"
}
```
And a `FetchDocument` indicates that the document has been created.
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message:
Content: {
  "_id": "emp2",
  "_rev": "1-8cfd1700eceb12adf67c55f96b43b6d0",
  "firstName": "Jon",
  "lastName": "Smith"
}
```
****

#####DeleteDocument
Deletes the document with id 'documentId' from the database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> DeleteDocument(string dbName, string documentId){..}
```
+ `"dbName"`: The name of the database.
+ `"documentId"`: The id of the document to be fetched.

######Usage: 
```csharp
Response<string> response = cDBLib.DeleteDocument("test-1", "emp1");
```

The response is:
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message: The document with documentId = emp1 was deleted.
Content: {
  "ok": true,
  "id": "emp1",
  "rev": "4-0a1ba0b5aa41f0a0a15b79f423b6cc2c"
}

```

And a `FetchDocument` indicates that the document has been deleted.
```csharp
Success: False, StatusCode: 404, ReasonPhrase: Object Not Found
Message: The Document with documentId = emp1 could not be found.
Content: {
  "error": "not_found",
  "reason": "deleted"
}
```
****

#####PostBulkDocument
Posts a bulk document to the database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> PostBulkDocument(string dbName, string document){..}
```
+ `"dbName"`: The name of the database.
+ `"document"`: The document to be posted to the database. Must be a JSON string in the format described [here](http://docs.couchdb.org/en/latest/api/database/bulk-api.html#db-bulk-docs).

######Usage: 
```csharp
//Post bulk doc
//{
//  "docs": [
//    {
//      "_id": "emp3",
//      "firstName": "John",
//      "lastName": "Doe",
//      "department": "HR"
//    },
//    {
//      "_id": "emp2",
//      "_rev": "1-8cfd1700eceb12adf67c55f96b43b6d0",
//      "firstName": "Jon",
//      "lastName": "Smith",
//      "department": "Accounts"
//    },
//    {
//      "_id": "cc5e82de2f03bf189828b3b390002161",
//      "_rev": "1-f9584b2364c83ae6e05c670e1c17eeb4",
//      "_deleted": true
//    }
//  ]
//}
//The first document will be created, the second document will be updated with the department information, the third document will be deleted.
string bulkdoc = "{\"docs\":[{\"_id\":\"emp3\",\"firstName\":\"John\",\"lastName\":\"Doe\",\"department\":\"HR\"},{\"_id\":\"emp2\",\"_rev\":\"1-8cfd1700eceb12adf67c55f96b43b6d0\",\"firstName\":\"Jon\",\"lastName\":\"Smith\",\"department\":\"Accounts\"},{\"_id\":\"cc5e82de2f03bf189828b3b390002161\",\"_rev\":\"1-f9584b2364c83ae6e05c670e1c17eeb4\",\"_deleted\":true}]}";
Response<string> response = cDBLib.PostBulkDocument("test-1", bulkdoc);
```

The response is:
```csharp
Success: True, StatusCode: 201, ReasonPhrase: Created
Message: The document was posted.
Please check the Content property for success or failure of operation on documen
ts.
Content: [
  {
    "ok": true,
    "id": "emp3",
    "rev": "1-20f7e9b1464ef5179cbc2fc3323c8517"
  },
  {
    "ok": true,
    "id": "emp2",
    "rev": "2-c8f9698e079824378654034908428da6"
  },
  {
    "ok": true,
    "id": "cc5e82de2f03bf189828b3b390002161",
    "rev": "2-6adb8452408c1aaa2a764146bfddc489"
  }
]
```
****

#####FetchDocuments
Fetches a collection of documents from the database 'dbName', in the queried instance of CouchDB, which matches a query.
```csharp
public Response<string> FetchDocuments(string dbName, string query){..}
```
+ `"dbName"`: The name of the database.
+ `"query"`: he query to be executed.

######Usage: 
```csharp
//condiser a design doc in the db test-1 with the view 'employees':
//{
//  "_id": "_design/employee",
//  "_rev": "1-f14e4609b5612178594dac4b1efa3984",
//  "views": {
//    "employees": {
//      "map": "function(doc) {
//                if(doc.department){
//                  emit(doc.department, null);
//                }
//              }",
//      "reduce": "_count"
//    }
//  },
//  "language": "javascript"
//}
//This allows us to query the data in many ways.
```
__Example: Get all the employees__ 
```csharp
string query = @"/_design/employee/_view/employees?include_docs=true&reduce=false";
Response<string> response = cDBLib.FetchDocuments("test-1", query);
```

Response:
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message:
Content: {
  "total_rows": 2,
  "offset": 0,
  "rows": [
    {
      "id": "emp2",
      "key": "Accounts",
      "value": null,
      "doc": {
        "_id": "emp2",
        "_rev": "2-c8f9698e079824378654034908428da6",
        "firstName": "Jon",
        "lastName": "Smith",
        "department": "Accounts"
      }
    },
    {
      "id": "emp3",
      "key": "HR",
      "value": null,
      "doc": {
        "_id": "emp3",
        "_rev": "1-20f7e9b1464ef5179cbc2fc3323c8517",
        "firstName": "John",
        "lastName": "Doe",
        "department": "HR"
      }
    }
  ]
}
```

__Example: Get all employees in a department, say HR__ 
```csharp
string query = @"/_design/employee/_view/employees?key=""HR""&include_docs=true&reduce=false";
Response<string> response = cDBLib.FetchDocuments("test-1", query);
```

Response:
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message:
Content: {
  "total_rows": 2,
  "offset": 1,
  "rows": [
    {
      "id": "emp3",
      "key": "HR",
      "value": null,
      "doc": {
        "_id": "emp3",
        "_rev": "1-20f7e9b1464ef5179cbc2fc3323c8517",
        "firstName": "John",
        "lastName": "Doe",
        "department": "HR"
      }
    }
  ]
}
```

__Example: Get the total count of employees__ 
```csharp
string query = @"/_design/employee/_view/employees";
Response<string> response = cDBLib.FetchDocuments("test-1", query);
```

Response:
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message:
Content: {
  "rows": [
    {
      "key": null,
      "value": 2
    }
  ]
}
```

__Example: Get the total count of employees in a department, say 'Accounts'__ 
```csharp
string query = @"/_design/employee/_view/employees?key=""Accounts""";
Response<string> response = cDBLib.FetchDocuments("test-1", query);
```

Response:
```csharp
Success: True, StatusCode: 200, ReasonPhrase: OK
Message:
Content: {
  "rows": [
    {
      "key": null,
      "value": 1
    }
  ]
}
```
****
