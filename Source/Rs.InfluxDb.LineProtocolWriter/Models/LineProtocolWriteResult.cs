namespace Rs.InfluxDb.LineProtocolWriter.Models
{
    public struct LineProtocolWriteResult
    {
        public LineProtocolWriteResult(bool success, string errorMessage = null)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; }

        public string ErrorMessage { get; }
    }
}