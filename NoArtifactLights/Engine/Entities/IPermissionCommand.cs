using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Engine.Entities
{
	internal interface IPermissionCommand
	{
		void ActualCode(object[] para);
		void ByRun(object[] para);
		IExecutor Executor { get; set; }
	}
}
