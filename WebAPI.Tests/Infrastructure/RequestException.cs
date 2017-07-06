using System;

namespace WebAPI.Tests.Infrastructure
{
    public class RequestException : Exception
    {
        public RequestException(string message)
            : base(message)
        {
        }
    }
}
