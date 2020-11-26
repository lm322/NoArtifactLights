using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandPlus.Commanding;
using NoArtifactLights.Engine.Entities.Enums;

namespace NoArtifactLights.Engine.Entities
{
	internal interface IExecutor
	{
		void Run(CommandControl ctrl, string command, Permission permission);
		Permission Permission { get; }
	}
}
