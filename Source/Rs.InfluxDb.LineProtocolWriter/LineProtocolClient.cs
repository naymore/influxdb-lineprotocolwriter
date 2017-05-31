using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Rs.InfluxDb.LineProtocolWriter.Models;

namespace Rs.InfluxDb.LineProtocolWriter
{
    public class LineProtocolClient : IDisposable
    {
        private readonly HttpClientHandler _httpClientHandler;
        private readonly HttpClient _httpClient;
        private readonly string _requestUri;
        private readonly bool _useGzipCompression;

        private bool _disposedValue;

        public LineProtocolClient(LineProtocolClientOptions lineProtocolClientOptions)
        {
            lineProtocolClientOptions.Validate();
            _useGzipCompression = lineProtocolClientOptions.UseGzipCompression;

            _httpClientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _httpClient = new HttpClient(_httpClientHandler) { BaseAddress = lineProtocolClientOptions.ServerBaseAddress };

            _requestUri = PrepareRequestUri(lineProtocolClientOptions);
        }

        // TODO: add retry logic?
        public async Task<LineProtocolWriteResult> WriteAsync(LineProtocolPayload lineProtocolPayload, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (lineProtocolPayload == null || lineProtocolPayload.Count == 0)
                return new LineProtocolWriteResult(true, "0", "No input.");

            HttpResponseMessage response;
            using (HttpContent content = CreateHttpContent(lineProtocolPayload))
            {
                response = await PostToLineProtocolEndpoint(_requestUri, content, cancellationToken);
            }

            string httpStatusCode = ((int)response.StatusCode).ToString();

            if (response.IsSuccessStatusCode)
                return new LineProtocolWriteResult(true, httpStatusCode);

            string errorMessage = await response.Content.ReadAsStringAsync();

            return new LineProtocolWriteResult(false, httpStatusCode, errorMessage);
        }

        protected virtual HttpContent CreateHttpContent(LineProtocolPayload lineProtocolPayload)
        {
            string requestContent;
            using (StringWriter stringWriter = new StringWriter())
            {
                lineProtocolPayload.Format(stringWriter);
                requestContent = stringWriter.ToString();
            }

            StringContent httpStringContent = new StringContent(requestContent);

            if (!_useGzipCompression)
                return httpStringContent;

            GzipHttpContent httpGzipContent = new GzipHttpContent(httpStringContent);
            return httpGzipContent;
        }

        protected virtual Task<HttpResponseMessage> PostToLineProtocolEndpoint(string endpoint, HttpContent httpContent, CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage> responseTask = _httpClient.PostAsync(endpoint, httpContent, cancellationToken);

            return responseTask;
        }

        private string PrepareRequestUri(LineProtocolClientOptions lineProtocolClientOptions)
        {
            string endpoint = $"write?db={Uri.EscapeDataString(lineProtocolClientOptions.DatabaseName)}";

            if (!string.IsNullOrEmpty(lineProtocolClientOptions.UserName))
            {
                endpoint += $"&u={Uri.EscapeDataString(lineProtocolClientOptions.UserName)}&p={Uri.EscapeDataString(lineProtocolClientOptions.Password)}";
            }

            return endpoint;
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                    _httpClientHandler?.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}