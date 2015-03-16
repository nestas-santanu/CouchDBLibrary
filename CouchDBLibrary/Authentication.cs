using System;
using System.Net;
using System.Net.Http.Headers;

namespace CouchDBLibrary
{
    public class Authentication
    {
        internal AuthenticationHeaderValue GetHeaderValues(AuthenticationSchemes authenticationScheme, string userName, string password)
        {
            AuthenticationHeaderValue headerValue = null;
            switch (authenticationScheme)
            {
                case AuthenticationSchemes.Basic:

                    headerValue = new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", userName, password))));
                    break;
            }

            return headerValue;
        }
    }
}
