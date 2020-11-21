using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotWorkshop.Flate.Ditno
{
    /// <summary>
    /// Represents a Ditno Client implementation.
    /// </summary>
    public interface IDitnoClient
    {
        /// <summary>
        /// Executes all <see cref="IDitnoTask"/> in this instance.
        /// </summary>
        void Execute();

        /// <summary>
        /// Adds an <see cref="IDitnoTask"/> into this instance.
        /// </summary>
        /// <param name="task">The task to add.</param>
        void AddInstance(IDitnoTask task);
    }
}
