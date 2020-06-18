using System;

namespace WhyNotEarth.Meredith.Exceptions
{
    public class InvalidActionException : Exception
    {
        public object? Error { get; set; }

        public InvalidActionException()
        {
        }

        public InvalidActionException(string message) : base(message)
        {
        }

        public InvalidActionException(object error)
        {
            Error = error;
        }
    }
}