using Diana.Core.Dtos;
using Diana.Server.Sessions;
using System.Net;
namespace Diana.Server.Messaging.Handlers;

class SetUdpEndPointHandler : IMessageHandler
{
    public async Task HandleMessage(ClientRequest msg, ClientSessionHandler clientHandler)
    {
        if (msg is null || msg.IpAddress is null)
            return;

        clientHandler.EndPoint = new IPEndPoint(IPAddress.Parse(msg.IpAddress), msg.port);

    }
}
