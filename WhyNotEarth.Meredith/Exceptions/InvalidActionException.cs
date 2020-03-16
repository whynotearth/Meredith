using System;

namespace WhyNotEarth.Meredith.Exceptions
{
    public class InvalidActionException : Exception
    {
        public InvalidActionException(string message) : base(message)
        {
        }
    }
}