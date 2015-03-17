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

#####ListDB:
Returns a list of DBs in the queried instance of CouchDB.
```csharp
public Response<string> ListDBs()
```
######Usage: 
```csharp
Response<string> response = cDBLib.ListDBs();
```




