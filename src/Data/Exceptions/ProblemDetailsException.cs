namespace ForexMiner.Heimdallr.Data.Exceptions
{
    using System;
    using System.Net;

    public class ProblemDetailsException : Exception
    {

        public HttpStatusCode Status { get; }

        public ProblemDetailsException() : base()
        {
            Status = HttpStatusCode.InternalServerError;
        }

        public ProblemDetailsException(HttpStatusCode status)
        {
            Status = status;
        }

        public ProblemDetailsException(HttpStatusCode status, string messsage) : base(messsage)
        {
            Status = status;
        }

    }
}
