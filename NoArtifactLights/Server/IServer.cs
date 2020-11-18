using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Server
{
	public interface IServer
	{
		void ConnectingWithoutIndex(NetworkClient client);
	    int Connecting(NetworkClient client);

		void Disconnect(int playerIndex);

		void UpdatePerferences<T>(int perferences, T value);
		void GetPerference(int perference);

		List<NetworkClient> Clients { get; }
	}
}
