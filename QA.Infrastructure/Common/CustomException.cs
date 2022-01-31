using System;

namespace QA.Infrastructure.Common
{
    public class CustomException : Exception
    {
        public CustomException(string message)
            : base(message) { }
    }
}
