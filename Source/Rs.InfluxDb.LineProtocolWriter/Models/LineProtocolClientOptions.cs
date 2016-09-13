using System;

namespace Rs.InfluxDb.LineProtocolWriter.Models
{
    public class LineProtocolClientOptions
    {
        public Uri ServerBaseAddress { get; set; }

        public string DatabaseName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool UseGzipCompression { get; set; }

        public int RequestAggregationMaxCount { get; set; } = 15000;

        public uint RequestAggregationMaxSeconds { get; set; } = 3;

        internal void Validate()
        {
            if (ServerBaseAddress == null)
                throw new ArgumentNullException(nameof(ServerBaseAddress));

            if (string.IsNullOrEmpty(DatabaseName))
                throw new ArgumentException("A database must be specified");
        }
    }
}