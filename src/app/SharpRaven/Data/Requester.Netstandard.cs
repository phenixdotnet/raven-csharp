#if NETSTANDARD

using Newtonsoft.Json;
using SharpRaven.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SharpRaven.Data
{
    /// <summary>
    /// The class responsible for performing the HTTP request to Sentry.
    /// </summary>
    public class Requester
    {
        private readonly RequestData data;
        private readonly JsonPacket packet;
        private readonly RavenClient ravenClient;
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="Requester"/> class.
        /// </summary>
        /// <param name="packet">The <see cref="JsonPacket"/> to initialize with.</param>
        /// <param name="ravenClient">The <see cref="RavenClient"/> to initialize with.</param>
        internal Requester(JsonPacket packet, RavenClient ravenClient)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (ravenClient == null)
                throw new ArgumentNullException(nameof(ravenClient));

            this.ravenClient = ravenClient;
            this.packet = ravenClient.PreparePacket(packet);
            this.data = new RequestData(this);

            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = ravenClient.CurrentDsn.SentryUri;
            this.httpClient.Timeout = ravenClient.Timeout;
            this.httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(ravenClient.CurrentDsn));
            this.httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(PacketBuilder.ProductName, PacketBuilder.Version));

            if (ravenClient.Compression)
            {
                this.httpClient.DefaultRequestHeaders.TransferEncoding.Add(new System.Net.Http.Headers.TransferCodingHeaderValue("gzip"));
            }
        }


        /// <summary>
        /// Gets the <see cref="IRavenClient"/>.
        /// </summary> 
        public IRavenClient Client
        {
            get { return this.ravenClient; }
        }

        /// <summary>
        /// 
        /// </summary>
        public RequestData Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// Gets the <see cref="JsonPacket"/> being sent to Sentry.
        /// </summary>
        public JsonPacket Packet
        {
            get { return this.packet; }
        }

        /// <summary>
        /// Executes the HTTP request to Sentry.
        /// </summary>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        public string Request()
        {
            // Run the async method on a new Thread to avoid deadlock on main thread
            var asyncTask = Task.Run(RequestAsync);
            asyncTask.Wait();

            return asyncTask.Result;
        }

        /// <summary>
        /// Executes the <c>async</c> HTTP request to Sentry.
        /// </summary>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        public async Task<string> RequestAsync()
        {
            HttpContent httpContent = null;
            if (this.ravenClient.Compression)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    StreamContent streamContent = new StreamContent(ms);
                    if (this.ravenClient.Compression)
                    {
                        streamContent.Headers.ContentEncoding.Add("gzip");
                        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    }
                    GzipUtil.Write(this.data.Scrubbed, ms);

                    httpContent = streamContent;
                }
            }
            else
            {
                StringContent stringContent = new StringContent(this.data.Scrubbed);
                stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                stringContent.Headers.ContentEncoding.Add("utf-8");

                httpContent = stringContent;
            }

            using (var response = await this.httpClient.PostAsync(string.Empty, httpContent).ConfigureAwait(false))
            {
                httpContent.Dispose();

                if (!response.IsSuccessStatusCode)
                {
                    string errorResponseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    SystemUtil.WriteError(errorResponseContent);
                    response.EnsureSuccessStatusCode();
                }

                string jsonContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseContent = JsonConvert.DeserializeObject<dynamic>(jsonContent);
                return responseContent.id;
            }
        }
    }
}


#endif