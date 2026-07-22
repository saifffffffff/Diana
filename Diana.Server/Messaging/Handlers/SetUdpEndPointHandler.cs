using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
using Diana.Server.Sessions;
using System.Net;
namespace Diana.Server.Messaging.Handlers;

class SetUdpEndPointHandler : IMessageHandler
{


    public async Task HandleMessageAsync(ClientRequest request, ISession clientHandler )
    {
        if (request is null || request.IpAddress is null)
            return;

        clientHandler!.EndPoint = new IPEndPoint(IPAddress.Parse(request.IpAddress), request.port);

    }
}
