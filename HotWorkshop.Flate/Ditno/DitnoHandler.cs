using HotWorkshop.Flate.Ditno.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotWorkshop.Flate.Ditno
{
    /// <summary>
    /// Represents a Ditno handler.
    /// Extends this instance to get better support.
    /// </summary>
    public abstract class DitnoHandler : IDitnoClient
    {
        public virtual List<IDitnoTask> Tasks { get; protected set; } = new List<IDitnoTask>();

        public int Interval { get; set; }

        public void AddInstance(IDitnoTask task)
        {
            if (Tasks.Contains(task)) throw new InvalidDitnoException(ExceptionTranslations.ExceptionDitnoAlreadyExists);
            Tasks.Add(task);
        }

        public virtual void Execute()
        {
            foreach(IDitnoTask task in Tasks)
            {
                task.Execute();
            }
        }
    }
}
