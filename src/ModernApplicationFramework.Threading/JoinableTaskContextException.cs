using System;

namespace ModernApplicationFramework.Threading
{
    [Serializable]
    public class JoinableTaskContextException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoinableTaskContextException"/> class.
        /// </summary>
        public JoinableTaskContextException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinableTaskContextException"/> class.
        /// </summary>
        /// <param name="message">The message for the exception</param>
        public JoinableTaskContextException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinableTaskContextException"/> class.
        /// </summary>
        /// <param name="message">The message for the exception</param>
        /// <param name="inner">The inner exception.</param>
        public JoinableTaskContextException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinableTaskContextException"/> class.
        /// </summary>
        protected JoinableTaskContextException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
