using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;

namespace Diana.Core.Abstractions.Messaging;


public interface IMessageHandler
{

    Task HandleMessageAsync(ClientRequest request, ISession clientHandler);
}

