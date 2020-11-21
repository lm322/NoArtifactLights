using System.Net;

namespace NoArtifactLights.Server
{
	public abstract class NetworkClient
	{
		public string Name { get; set; }
		public IPAddress IP { get; set; }
		IServer currentServer;
		int index;

		public virtual void ConnectTo(IServer server)
		{
			index = server.Connecting(this);
			currentServer = server;
		}

		public virtual void LeaveGracefully()
		{
			currentServer.Disconnect(index);
			Leave();
		}

		public abstract void LeaveForcefully();
		public abstract void Leave();
	}
}
