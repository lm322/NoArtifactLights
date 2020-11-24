using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandPlus.Commanding;
using CommandPlus.Exceptions;
using NoArtifactLights.Engine.Mod.Controller;

namespace NoArtifactLights.Engine.Mod.Commands
{
	internal class SetHungryCommand : Command
	{
		public SetHungryCommand()
		{
			this.ArgumentTypes.Add(typeof(float));
		}

		public override void Executed(object[] arguments)
		{
			float hungry = this.VerifyAndConstruct<float>(0, arguments[0]);

			if(hungry < 0 || hungry > 10)
			{
				throw new UnexceptedValueException(1, "hungry value", "non-hungry float");
			}

			HungryController.Hungry = hungry;
		}
	}
}
