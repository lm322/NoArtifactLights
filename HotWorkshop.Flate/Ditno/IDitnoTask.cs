using System;

namespace HotWorkshop.Flate.Ditno
{
    /// <summary>
    /// Represents a Ditno task.
    /// </summary>
    public interface IDitnoTask
    {
        /// <summary>
        /// Executes this instance.
        /// </summary>
        void Execute();

        /// <summary>
        /// Querys the current exception caught of this instance.
        /// </summary>
        /// <returns></returns>
        Exception GetCurrentFailure();

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        bool IsActive { get; }
    }
}
