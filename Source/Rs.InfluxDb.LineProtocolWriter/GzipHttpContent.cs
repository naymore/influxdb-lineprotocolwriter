using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rs.InfluxDb.LineProtocolWriter
{
    public class GzipHttpContent : HttpContent
    {
        private readonly HttpContent _originalContent;

        public GzipHttpContent(HttpContent content) : base()
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            _originalContent = content;

            foreach (KeyValuePair<string, IEnumerable<string>> header in _originalContent.Headers)
            {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            Headers.ContentEncoding.Add("gzip");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            GZipStream gzipStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true);

            return _originalContent.CopyToAsync(gzipStream).ContinueWith(task =>
            {
                gzipStream?.Dispose();
            });

        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;

            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _originalContent?.Dispose();
            }
            
            base.Dispose(disposing);
        }
    }
}