using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Http;
using Telerik.Sitefinity.TestIntegration.Helpers;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations
{
    /// <summary>
    /// Provides common web requests that need authentication
    /// </summary>
    public class FeatherWebRequestsOperations
    {
        /// <summary>
        /// Make Authenticate GET Request
        /// </summary>
        /// <param name="url">The web request url </param>
        /// <returns>The response from the request</returns>
        public HttpResponseMessage MakeAuthenticateGetRequest(string url)
        {
            var client = new SitefinityClient();
            client.RequestAuthenticate();
            return client.Send(HttpMethod.GET, url);
        }

        /// <summary>
        /// Gets the response content from GET Request
        /// </summary>
        /// <param name="url">The web request url </param>
        /// <returns>The content from the response</returns>
        public string GetResponseContentFromAuthenticateGetRequest(string url)
        {
            var response = this.MakeAuthenticateGetRequest(url);
            return response.Content.ReadAsString();
        }

        /// <summary>
        /// Make Authenticate PUT Request
        /// </summary>
        /// <param name="url">The web request url </param>
        /// <param name="payload">The payload of the request</param>
        public void MakeAuthenticatePutRequest(string url, string payload)
        {
            var client = new SitefinityClient();
            client.RequestAuthenticate();
            var request = new HttpRequestMessage("put", url);

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(payload);
            request.Headers.ContentType = "text/json";
            request.Headers.ContentLength = bytes.Length;
            request.Content = HttpContent.Create(bytes);
            client.Send(request);
        }
    }
}
