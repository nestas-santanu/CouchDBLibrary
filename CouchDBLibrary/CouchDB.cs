using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net;

namespace CouchDBLibrary
{
    public class CouchDB
    {
        private string baseUrl;
        private string userName;
        private string password;
        private AuthenticationSchemes authenticationScheme;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseUrl">Defines the instance of the CouchDB being queried. Default is http://127.0.0.1:5984</param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public CouchDB( AuthenticationSchemes authenticationScheme, string userName, string password, string baseUrl = @"http://127.0.0.1:5984")
        {
            this.baseUrl = baseUrl;
            this.userName = userName;
            this.password = password;
            this.authenticationScheme = authenticationScheme;
        }

        public CouchDB(string baseUrl = @"http://127.0.0.1:5984")
        {
            this.baseUrl = baseUrl;
            this.userName = "";
            this.password = "";
            this.authenticationScheme = AuthenticationSchemes.None;
        }

        /// <summary>
        /// Creates a database in the queried instance of CouchDB.
        /// </summary>
        /// <param name="dbName">The name of the database to be created. The database name must be composed of one or more of the following characters:
        /// 1. Lowercase characters (a-z)
        /// 2. Name must begin with a lowercase letter
        /// 3. Digits (0-9)
        /// 4. Any of the characters _, $, (, ), +, -, and /.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the success/ failure JSON statement returned by CouchDB.</returns>
        public Response<string> CreateDB(string dbName)
        {
            //===================================================================================================
            //A database is ceated by a PUT to CouchDB.
            //curl -X PUT -i http://127.0.0.1:5984/dbName

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the database does not exist it will be created, and CouchDB will return a confirmatiion.

            //HTTP/1.1 201 Created
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Location: http://127.0.0.1:5984/dbName
            //Date: Thu, 15 Jan 2015 12:54:29 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 12
            //Cache-Control: must-revalidate

            //{"ok":true}

            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the database does exist and CouchDB will return a 'file_exists' error.

            //HTTP/1.1 412 Precondition Failed
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Thu, 15 Jan 2015 13:03:19 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 95
            //Cache-Control: must-revalidate

            //{"error":"file_exists","reason":"The database could not be created, the file already exists."}
            //===================================================================================================

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    HttpContent content = new StringContent("");
                    httpResponse = client.PutAsync(@"/" + dbName, content).Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);
                    response.Message = "The database was created at " + baseUrl + @"/" + dbName + ".";

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                switch ((int)httpResponse.StatusCode)
                {
                    case 400:
                        response.Message = "Invalid database name.";
                        break;
                    case 412:
                        response.Message = "The database " + baseUrl + "/" + dbName + " exists.\n";
                        break;
                    default:
                        response.Message = e.Message;
                        break;
                }

                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nCould not create the database at " + baseUrl + @"/" + dbName + ".\n" + response.Message;

                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Returns a list of DBs in the queried instance of CouchDB.
        /// </summary>
        /// <returns>Returns a Response object. Access the Content property of the Response object for a list of all the databases in the queried instance of CouchDB.</returns>
        public Response<string> ListDBs()
        {
            //===================================================================================================
            //returns a list of DBs in queried instance of CouchDB. This is done by a GET to CouchDB.
            //curl -i http://127.0.0.1:5984/_all_dbs

            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //CouchDb will return a list of databases in CouchDB

            //HTTP/1.1 200 OK
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Sun, 15 Mar 2015 07:50:40 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 220
            //Cache-Control: must-revalidate

            //["_replicator","_users","database_test-1","database_test-2"]
            //===================================================================================================

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    httpResponse = client.GetAsync(@"/_all_dbs").Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);
                    response.Message = "The Content property contains the list of databases in the CouchDB at " + baseUrl + ".";

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Checks if the database exists in the queried instance of CouchDB. Returns its information, if it exists.
        /// </summary>
        /// <param name="dbName">The name of the database being queried.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for information on the queried database. The information is a JSON statement returned by CouchDB.</returns>
        public Response<string> FetchDB(string dbName)
        {
            //===================================================================================================
            //checks if the database exists. Returns its information, if it exisys. This is done by a GET to CouchDB.
            //curl -i http://127.0.0.1:5984/dbName

            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the database exists, CouchDb will return details of the database

            //HTTP/1.1 200 OK
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Sun, 18 Jan 2015 08:51:44 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 246
            //Cache-Control: must-revalidate

            //{"db_name":"geodb-country","doc_count":250,"doc_del_count":0,"update_seq":250,"purge_seq":0,"compact_running":false,"disk_size":454760,"data_size":74110,
            //"instance_start_time":"1421571058147480","disk_format_version":6,"committed_update_seq":250}

            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the database does not exist, CouchDB will return a 'not_found' error.

            //HTTP/1.1 404 Object Not Found
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Sun, 18 Jan 2015 08:52:17 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 44
            //Cache-Control: must-revalidate

            //{"error":"not_found","reason":"no_db_file"}
            //===================================================================================================

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    httpResponse = client.GetAsync(@"/" + dbName).Result;
                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);
                    response.Message = "The database exists. The Content property contains the information of the database " + dbName + ".";

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);

                switch ((int)httpResponse.StatusCode)
                {   
                    case 404:
                        response.Message = "The database " + dbName + " was not found.";
                        break;
                    default:
                        response.Message = e.Message;
                        break;
                }

                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Compacts the database 'dbName' in the queried instance of CouchDB.
        /// </summary>
        /// <param name="dbName">The name of the database to compact.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the success/ failure JSON statement returned by CouchDB.</returns>
        public Response<string> CompactDatabase(string dbName)
        {
            //===================================================================================================
            //curl -i -H "Content-Type: application/json" -X POST http://localhost:5984/dbName/_compact

            //HTTP/1.1 202 Accepted
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Wed, 11 Mar 2015 16:34:30 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 12
            //Cache-Control: must-revalidate

            //{"ok":true}
            //===================================================================================================

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    HttpContent content = new StringContent("", UTF8Encoding.UTF8, "application/json");
                    httpResponse = client.PostAsync(@"/" + dbName + @"//_compact", content).Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);
                    response.Message = "The request for compaction of the database" + dbName + " has been accepted.";

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nCould not accept the request for compacting the database " + dbName + ".\n" + response.Message;
                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Compacts the views in the design document 'designDoc' in the database 'dbName' in the queried instance of CouchDB.
        /// </summary>
        /// <param name="dbName">The name of the database being queried.</param>
        /// <param name="designDoc">The name of the design document.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the success/ failure JSON statement returned by CouchDB.</returns>
        public Response<string> CompactView(string dbName, string designDoc)
        {
            //===================================================================================================
            //curl -i -H "Content-Type: application/json" -X POST http://localhost:5984/dbName/_compact/designDocName

            //HTTP/1.1 202 Accepted
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Wed, 11 Mar 2015 16:34:30 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 12
            //Cache-Control: must-revalidate

            //{"ok":true}

            //===================================================================================================

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    HttpContent content = new StringContent("", UTF8Encoding.UTF8, "application/json");
                    httpResponse = client.PostAsync(@"/" + dbName + @"/_compact/" + designDoc, content).Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);
                    response.Message = "The request for compaction of the views in design document " + designDoc + " in the database " + dbName + " has been accepted.";

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nfor compaction of the views in design document " + designDoc + " in the database " + dbName + ".\n" + response.Message;
                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Deletes the database 'dbName' in the queried instance of CouchDB.
        /// </summary>
        /// <param name="dbName">The name of the database to be deleted.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the success/ failure JSON statement returned by CouchDB.</returns>
        public Response<string> DeleteDB(string dbName)
        {
            //===================================================================================================
            //A database is deleted by a DELETE to CouchDB.
            //curl -i  -X DELETE http://127.0.0.1:5984/dbName

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the database is deleted, CouchDB will return a confirmatiion.

            //HTTP/1.1 200 OK
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Fri, 30 Jan 2015 13:17:37 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 12
            //Cache-Control: must-revalidate

            //{"ok":true}

            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the database does not exist and CouchDB will return a 'missing' error.

            //HTTP/1.1 404 Object Not Found
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Fri, 30 Jan 2015 13:16:43 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 41
            //Cache-Control: must-revalidate

            //{"error":"not_found","reason":"missing"}        
            //===================================================================================================

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    httpResponse = client.DeleteAsync(@"/" + dbName).Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);
                    response.Message = "The database at " + baseUrl + @"/" + dbName + " was deleted.";

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                if (response.StatusCode == 404)
                {
                    response.Message = "The database at " + baseUrl + "/" + dbName + " could not be found.\n" + e.Message;
                }
                else
                {
                    response.Message = e.Message;
                }

                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nCould not delete the database at " + baseUrl + "/" + dbName + ".\n" + response.Message;
                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Creates a document in the database 'dbName' in the queried instance of CouchDB.
        /// </summary>
        /// <param name="dbName">The name of the database being queried.</param>
        /// <param name="document">The document to be inserted. This document must be a JSON string.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the success/ failure JSON statement returned by CouchDB.
        /// In case of success, the JSON statement will contain the 'id' and 'revision id' of the document created. </returns>
        public Response<string> CreateDocument(string dbName, string document)
        {
            //===================================================================================================
            //A document is ceated by a POST to CouchDB.
            //curl -i -X PUT -d @"C:\Shared Folder\IN.json"  -H "Content-type:application/json; charset=UTF-8" http://192.168.1.6:5984/dbName
            //This will create a document in the dbName database
            //also see: http://nestas.blogspot.in/2015/01/error-using-curl-to-post-or-put-data.html
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the document does not exist it will be created, and CouchDB will return a confirmatiion.

            //HTTP/1.1 201 Created
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Location: http://127.0.0.1:5984/geodb-country
            //Date: Sun, 18 Jan 2015 15:55:02 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 65
            //Cache-Control: must-revalidate

            //{"ok":true,"id":"1-8d942f687cc9a582b25f5b57674f3240","rev":"1-7d942f687cc9a582b25f5b57674f3240"}
            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the document does exist and CouchDB will return a 'conflict' error.

            //HTTP/1.1 409 Conflict
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Sun, 18 Jan 2015 15:22:48 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 58
            //Cache-Control: must-revalidate

            //{"error":"conflict","reason":"Document update conflict."}
            //===================================================================================================

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    HttpContent content = new StringContent(document, UTF8Encoding.UTF8, "application/json");
                    httpResponse = client.PostAsync(@"/" + dbName, content).Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);
                    response.Message = "The document was created.";

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nCould not create the document.\n" + response.Message;
                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Creates a document with id as 'documentId' in the database 'dbName' in the queried instance of CouchDB.
        /// </summary>
        /// <param name="dbName">The name of the database being queried.</param>
        /// <param name="documentId">The id of the document.</param>
        /// <param name="document">The document to be inserted. This document must be a JSON string.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the success/ failure JSON statement returned by CouchDB.
        /// In case of success, the JSON statement will contain the 'id' and 'revision id' of the document created. </returns>
        public Response<string> CreateDocument(string dbName, string documentId, string document)
        {
            //===================================================================================================
            //A document with a pre-defined id is ceated by a PUT to CouchDB.
            //curl -i -X PUT -d @"C:\Shared Folder\IN.json"  -H "Content-type:application/json; charset=UTF-8" http://192.168.1.6:5984/geodb-country/IN
            //This will create a document with id = "IN" in the geodb-country database
            //also see: http://nestas.blogspot.in/2015/01/error-using-curl-to-post-or-put-data.html
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the document does not exist it will be created, and CouchDB will return a confirmatiion.

            //HTTP/1.1 201 Created
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Location: http://127.0.0.1:5984/geodb-country
            //Date: Sun, 18 Jan 2015 15:55:02 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 65
            //Cache-Control: must-revalidate

            //{"ok":true,"id":"IN","rev":"1-7d942f687cc9a582b25f5b57674f3240"}
            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the document does exist and CouchDB will return a 'conflict' error.

            //HTTP/1.1 409 Conflict
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Sun, 18 Jan 2015 15:22:48 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 58
            //Cache-Control: must-revalidate

            //{"error":"conflict","reason":"Document update conflict."}
            //===================================================================================================

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    HttpContent content = new StringContent(document, UTF8Encoding.UTF8, "application/json");
                    httpResponse = client.PutAsync(@"/" + dbName + @"/" + documentId, content).Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);
                    response.Message = "The document with id = " + documentId + " created.";

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                if (response.StatusCode == 409)
                {
                    response.Message = "The document with id = " + documentId + "already exists.";
                }
                else
                {
                    response.Message = e.Message;
                }

                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nCould not insert the document with id " + documentId + ".\n" + response.Message;
                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Fetches the document with id 'documentId' from the database 'dbName' in the queried instance of CouchDB.
        /// </summary>
        /// <param name="dbName">The name of the database being queried.</param>
        /// <param name="documentId">The id of the document to be fetched.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the fetched document or the failure JSON statement returned by CouchDB.</returns>
        public Response<string> FetchDocument(string dbName, string documentId)
        {
            //===================================================================================================
            //A document is retrieved by a PUT to CouchDB.
            //curl -i http://127.0.0.1:5984/geodb-country/IN
            //This will retrieve a document with id = "IN" in the geodb-country database
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the document exists CouchDB will return the document.

            //HTTP/1.1 200 OK
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //ETag: "1-d4c8b330d781b982184e0e6829f434cd"
            //Date: Sun, 18 Jan 2015 16:01:52 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 214
            //Cache-Control: must-revalidate

            //{"_id":"IN","_rev":"1-d4c8b330d781b982184e0e6829f434cd","Continent":"AS","ContinentName":"Asia","CountryName":"India","Capital":"New Delhi",
            //"Iso_Alpha2":"IN","Iso_Numeric":"356","Iso_Alpha3":"IND","FipsCode":"IN"}
            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //if the document does not exist, CouchDB will return a 'not_found' error.

            //HTTP/1.1 404 Object Not Found
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Sun, 18 Jan 2015 16:04:39 GMT
            //Content-Type: text/plain; charset=utf-8
            //Content-Length: 41
            //Cache-Control: must-revalidate

            //{"error":"not_found","reason":"missing"}            
            //===================================================================================================

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    httpResponse = client.GetAsync(@"/" + dbName + @"/" + documentId).Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                if (response.StatusCode == 404)
                {
                    response.Message = "The Document with document id = " + documentId + "could not be found.\n" + e.Message;
                }
                else
                {
                    response.Message = e.Message;
                }

                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nCould not retrieve the document with document id " + documentId + ".\n" + response.Message;
                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Updates a document with 'documentId' in the database 'dbName' in the queried instance of CouchDB.
        /// </summary>
        /// <param name="dbName">The name of the database being queried.</param>
        /// <param name="documentId">The id of the document to be updated.</param>
        /// <param name="revisionId">The revision of the document to be updated.</param>
        /// <param name="document">The document to be updated. The document must be a JSON statement.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the success/ failure JSON statement returned by CouchDB.</returns>
        public Response<string> UpdateDocument(string dbName, string documentId, string revisionId, string document)
        {

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    HttpContent content = new StringContent(document, UTF8Encoding.UTF8, "application/json");
                    httpResponse = client.PutAsync(@"/" + dbName + @"/" + documentId + "?rev=" + revisionId, content).Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);
                    response.Message = "The document with document ID = " + documentId + " updated.";

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                if (response.StatusCode == 404)
                {
                    response.Message = "The Document with document id = " + documentId + "could not be found.\n" + e.Message;
                }
                else
                {
                    response.Message = e.Message;
                }

                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nCould not update the document with document id " + documentId + ".\n" + response.Message;

                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Checks for a document with id 'documentId' in the database 'dbName' in the queried instance of CouchDB. If not found, creates the document; if found, updates the latest revision of the document.
        /// </summary>
        /// <param name="dbName">The name of the database being queried.</param>
        /// <param name="documentId">The id of the document.</param>
        /// <param name="document">The document to be created/ updated. The document must be a JSON statement.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the success/ failure JSON statement returned by CouchDB.</returns>
        public Response<string> UpsertDocument(string dbName, string documentId, string document)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                Response<string> response = FetchDocument(dbName, documentId);
                var data = JObject.Parse(response.Content);

                switch (response.StatusCode)
                {
                    case 200: //document exists, update document
                        var revisionId = data["_rev"].ToString();
                        response = UpdateDocument(dbName, documentId, revisionId, document);
                        break;
                    case 404: // document not available, insert document
                        response = CreateDocument(dbName, documentId, document);
                        break;
                    default:
                        break;
                }

                return response;
            }
        }

        /// <summary>
        /// Deletes a document with the id 'documentId' in the database'dbName' in the queried instance of CouchDB.
        /// </summary>
        /// <param name="dbName">The name of the database being queried.</param>
        /// <param name="documentId">The id of the document to be deleted</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the success/ failure JSON statement returned by CouchDB.</returns>
        public Response<string> DeleteDocument(string dbName, string documentId)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                Response<string> response = FetchDocument(dbName, documentId);

                if (response.StatusCode == 200)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var data = JObject.Parse(response.Content);
                        var revisionId = data["_rev"].ToString();

                        client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                        client.BaseAddress = new Uri(baseUrl);

                        httpResponse = client.DeleteAsync(@"/" + dbName + "/" + documentId + "?rev=" + revisionId).Result;

                        resultString = httpResponse.Content.ReadAsStringAsync().Result;

                        httpResponse.EnsureSuccessStatusCode();

                        response = CreateResponse(httpResponse, resultString);
                        response.Message = "The document " + documentId + " was deleted.";

                        return response;
                    }
                }
                else
                {
                    return response;
                }

            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                if (response.StatusCode == 400)
                {
                    response.Message = "The document " + documentId + " could not be found.\n" + e.Message;
                }
                else
                {
                    response.Message = e.Message;
                }

                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nCould not delete the document " + documentId + ".\n" + response.Message;

                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }

        }

        /// <summary>
        /// Posts a bulk document to the database 'dbName' in the queried instance of CouchDB. 
        /// </summary>
        /// <param name="dbName">The name of the database.</param>
        /// <param name="document">Th documentto beposted. The document must be in a JSON format.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the success/ failure JSON statement returned by CouchDB for operation on each individual document.</returns>
        public Response<string> PostBulkDocument(string dbName, string document)
        {
            //===================================================================================================
            //Documents are inserted in bulk by a POST to CouchDB.
            //http://docs.couchdb.org/en/latest/api/database/bulk-api.html#db-bulk-docs
            //curl -i -X POST -d @"C:/Shared Folder/BulkDocs.json" -H "Accept:application/json" -H "Content-type:application/json" http://192.168.1.6:5984/dbName/_bulk_docs

            //HTTP/1.1 201 Created
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Fri, 23 Jan 2015 12:34:53 GMT
            //Content-Type: application/json
            //Content-Length: 239
            //Cache-Control: must-revalidate

            //[{"ok":true,"id":"FishStew","rev":"1-9c65296036141e575d32ba9c034dd3ee"},
            //{"ok":true,"id":"LambStew","rev":"1-34c318924a8f327223eed702ddfdc66d"},
            //{"ok":true,"id":"e7f44f2f4189e1243d4e3c4e68000de0","rev":"1-857c7cbeb6c8dd1dd34a0c73e8da3c44"}]

            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            //HTTP/1.1 201 Created
            //Server: CouchDB/1.6.1 (Erlang OTP/R16B02)
            //Date: Fri, 23 Jan 2015 12:40:45 GMT
            //Content-Type: application/json
            //Content-Length: 245
            //Cache-Control: must-revalidate

            //[{"id":"FishStew","error":"conflict","reason":"Document update conflict."},
            //{"id":"LambStew","error":"conflict","reason":"Document update conflict."},
            //{"ok":true,"id":"e7f44f2f4189e1243d4e3c4e680012e9","rev":"1-857c7cbeb6c8dd1dd34a0c73e8da3c44"}]
            //===================================================================================================

            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    HttpContent content = new StringContent(document, UTF8Encoding.UTF8, "application/json");
                    httpResponse = client.PostAsync(@"/" + dbName + @"//_bulk_docs", content).Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);
                    response.Message = "Bulk inserion of documents done.\nPlease check the Content property for success or failure of operation on documents.";

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured. Could not post the document.";

                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nCould not post the document.\n" + response.Message;
                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

        /// <summary>
        /// Fetches a collection of documents from the database 'dbName' in the queried instance of CouchDB which matches the queryParameters.
        /// </summary>
        /// <param name="dbName">The name of the database.</param>
        /// <param name="queryParameters">The query parameter(s) to be executed.</param>
        /// <returns>Returns a Response object. Access the Content property of the Response object for the fetched documents/ failure JSON statement returned by CouchDB</returns>
        public Response<string> FetchDocuments(string dbName, string queryParameters)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            string resultString = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(userName, password, authenticationScheme);
                    client.BaseAddress = new Uri(baseUrl);

                    httpResponse = client.GetAsync(@"/" + dbName + queryParameters).Result;

                    resultString = httpResponse.Content.ReadAsStringAsync().Result;

                    httpResponse.EnsureSuccessStatusCode();

                    Response<string> response = CreateResponse(httpResponse, resultString);

                    return response;
                }
            }
            catch (HttpRequestException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
            catch (System.AggregateException e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                response.Message = "An error occured.\nCould not fetch the documents.\n" + response.Message;
                return response;
            }
            catch (Exception e)
            {
                Response<string> response = CreateResponse(httpResponse, e, resultString);
                return response;
            }
        }

       
        private AuthenticationHeaderValue GetAuthenticationHeaderValue(string userName, string password, AuthenticationSchemes authType)
        {
            Authentication authentication = new Authentication();
            var headerValue = authentication.GetHeaderValues(authType, userName, password);

            return headerValue;
        }

        private Response<string> CreateResponse(HttpResponseMessage httpResponse, string resultString)
        {
            Response<string> response = new Response<string>
            {
                Success = httpResponse.IsSuccessStatusCode,
                StatusCode = (int)httpResponse.StatusCode,
                ReasonPhrase = httpResponse.ReasonPhrase,
                Message = "",
                Content = GetContent(resultString)
            };

            return response;
        }

        private Response<string> CreateResponse(HttpResponseMessage httpResponse, HttpRequestException e, string resultString)
        {
            Response<string> response = new Response<string>
            {
                Success = httpResponse.IsSuccessStatusCode,
                StatusCode = (int)httpResponse.StatusCode,
                ReasonPhrase = httpResponse.ReasonPhrase,
                Message = e.Message,
                Content = GetContent(resultString)
            };

            return response;
        }

        private Response<string> CreateResponse(HttpResponseMessage httpResponse, AggregateException e, string resultString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Exception ex in e.InnerExceptions)
            {
                sb.Append(ex.InnerException.Message + ". ").AppendLine();
            }

            Response<string> response = new Response<string>
            {
                Success = false,
                StatusCode = (int)httpResponse.StatusCode,
                ReasonPhrase = httpResponse.ReasonPhrase,
                Message = sb.ToString(),
                Content = GetContent(resultString)
            };

            return response;
        }

        private Response<string> CreateResponse(HttpResponseMessage httpResponse, Exception e, string resultString)
        {
            Response<string> response = new Response<string>
            {
                Success = httpResponse.IsSuccessStatusCode,
                StatusCode = (int)httpResponse.StatusCode,
                ReasonPhrase = httpResponse.ReasonPhrase,
                Message = e.Message,
                Content = GetContent(resultString)
            };

            return response;
        }

        private string GetContent(string resultString)
        {
            JToken token = JToken.Parse(resultString);
            string content = string.Empty;

            switch(token.Type)
            {
                case JTokenType.Array:
                    content = JArray.Parse(resultString).ToString();
                    break;

                case JTokenType.Object:
                    content = JObject.Parse(resultString).ToString();
                    break;

                case JTokenType.Null:
                    content = "";
                    break;

                default:
                    content = "";
                    break;
            }

            return content;
        }
    }
}

