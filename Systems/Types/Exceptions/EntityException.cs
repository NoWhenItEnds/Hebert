using System;

namespace Hebert.Types.Exceptions
{
    /// <summary> An exception thrown by an entity. </summary>
    public class EntityException : Exception
    {
        /// <summary> An exception thrown by an entity. </summary>
        public EntityException() { }


        /// <summary> An exception thrown by an entity. </summary>
        /// <param name="message"> The exception's error message. </param>
        public EntityException(String message) : base(message) { }


        /// <summary> An exception thrown by an entity. </summary>
        /// <param name="message"> The exception's error message. </param>
        /// <param name="innerException"> The exception wrapped by this one. </param>
        public EntityException(String message, Exception innerException) : base(message, innerException) { }
    }
}
