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

####Methods:
+ [CreateDB](https://github.com/nestas-santanu/CouchDBLibrary/blob/master/README.md#createdb)
+ [ListDBs](https://github.com/nestas-santanu/CouchDBLibrary/blob/master/README.md#listdbs)
+ [FetchDB](https://github.com/nestas-santanu/CouchDBLibrary/blob/master/README.md#fetchdb)
+ [CompactDatabase](https://github.com/nestas-santanu/CouchDBLibrary/blob/master/README.md#compactdatabase)
+ [CompactView](https://github.com/nestas-santanu/CouchDBLibrary/blob/master/README.md#compactview)
+ [DeleteDB](https://github.com/nestas-santanu/CouchDBLibrary/blob/master/README.md#deletedb)
+ [CreateDocument](https://github.com/nestas-santanu/CouchDBLibrary/blob/master/README.md#createdocument)
+ FetchDocument
+ UpdateDocument
+ UpsertDocument
+ DeleteDocument
+ PostBulkDocument
+ FetchDocuments

#####CreateDB: 
Creates a database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> CreateDB(string dbName){..}
```
+ `"dbName"`: The name of the database to be created.

######Usage: 
```csharp
Response<string> response = cDBLib.CreateDB("test-1");
```

#####ListDBs:
Returns a list of DBs in the queried instance of CouchDB.
```csharp
public Response<string> ListDBs(){..}
```
######Usage: 
```csharp
Response<string> response = cDBLib.ListDBs();
```

#####FetchDB:
Checks if the database exists in the queried instance of CouchDB. Returns its information, if it exists.
```csharp
public Response<string> FetchDB(string dbName){..}
```
+ `"dbName"`: The name of the database.

######Usage: 
```csharp
Response<string> response = cDBLib.FetchDB("test-1");
```

#####CompactDatabase:
Compacts the database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> CompactDatabase(string dbName){..}
```
+ `"dbName"`: The name of the database to compact.

######Usage: 
```csharp
Response<string> response = cDBLib.CompactDatabase("test-1");
```

#####CompactView:
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

#####DeleteDB:
Deletes the database 'dbName' in the queried instance of CouchDB.
```csharp
public Response<string> DeleteDB(string dbName){..}
```
+ `"dbName"`: The name of the database to delete.

######Usage: 
```csharp
Response<string> response = cDBLib.DeleteDB("test-1");
```

#####CreateDocument:
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
Success: True
StatusCode: 201
ReasonPhrase: Created
Message: The document was created.
Content: {
  "ok": true,
  "id": "7102b1416523f7398a90cf8120000611",
  "rev": "1-f9584b2364c83ae6e05c670e1c17eeb4"
}
```

#####CreateDocument - overloaded:
Creates a document in the database 'dbName' with the id 'documentId'in the queried instance of CouchDB.
```csharp
public Response<string> CreateDocument(string dbName, string documentId, string document){..}
```
+ `"dbName"`: The name of the database.
+ `"dbName"`: The id of the document to be created.
+ `"document"`: The document to be created in the database. Must be a JSON string.

######Usage: 
```csharp
string json = string json = "{\"firstName\":\"John\",\"lastName\":\"Doe\"}";;
Response<string> response = cDBLib.CreateDocument("test-1", "emp1", json);
```

The response is:
```csharp
Success: True
StatusCode: 201
ReasonPhrase: Created
Message: The document with id = emp1 created.
Content: {
  "ok": true,
  "id": "emp1",
  "rev": "1-f9584b2364c83ae6e05c670e1c17eeb4"
}
```








