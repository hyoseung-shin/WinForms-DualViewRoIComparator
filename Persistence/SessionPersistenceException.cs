using System;

namespace DualViewRoiComparator.Persistence
{
    /// <summary>
    /// Raised by <see cref="SessionManager"/> for validation or storage failures that should
    /// be reported to the user (e.g. blank/duplicate name, write-permission problems). The
    /// message is user-facing (Korean) and safe to show directly in a MessageBox.
    /// </summary>
    [Serializable]
    public class SessionPersistenceException : Exception
    {
        public SessionPersistenceException(string message) : base(message) { }

        public SessionPersistenceException(string message, Exception inner) : base(message, inner) { }

        protected SessionPersistenceException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
