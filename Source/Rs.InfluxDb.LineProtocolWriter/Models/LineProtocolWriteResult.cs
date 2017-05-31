namespace Rs.InfluxDb.LineProtocolWriter.Models
{
    public struct LineProtocolWriteResult
    {
        public LineProtocolWriteResult(bool success, string httpStatusCode, string errorMessage = null)
        {
            Success = success;
            ErrorMessage = errorMessage;
            HttpStatusCode = httpStatusCode;
        }

        public bool Success { get; }

        public string HttpStatusCode { get; }

        public string ErrorMessage { get; }
    }
}