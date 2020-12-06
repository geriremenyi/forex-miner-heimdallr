//----------------------------------------------------------------------------------------
// <copyright file="ProblemDetailsException.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Exceptions
{
    using System;
    using System.Net;

    /// <summary>
    /// Problem details exception
    /// </summary>
    public class ProblemDetailsException : Exception
    {
        /// <summary>
        /// HTTP status code of the exception 
        /// </summary>
        public HttpStatusCode Status { get; }

        /// <summary>
        /// Empty constructor.
        /// Initializes the exception with HTTP 500 as the status messsage.
        /// </summary>
        public ProblemDetailsException() : base()
        {
            Status = HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Constructor with status.
        /// Initializes the exception with the given status.
        /// </summary>
        /// <param name="status"></param>
        public ProblemDetailsException(HttpStatusCode status)
        {
            Status = status;
        }

        /// <summary>
        /// Constructor with status and message.
        /// Initializes the exception with status code and message.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="messsage"></param>
        public ProblemDetailsException(HttpStatusCode status, string messsage) : base(messsage)
        {
            Status = status;
        }

    }
}
