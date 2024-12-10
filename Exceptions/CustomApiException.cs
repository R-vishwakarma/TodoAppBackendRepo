
namespace TodoApp.Exceptions
{
    public class CustomApiException : Exception
    {
        public int StatusCode { get; set; }

        public CustomApiException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public CustomApiException(string message, int statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}


