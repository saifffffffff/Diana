using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging;

interface IMessageHandler
{
    Task HandleMessage(ClientRequest msg , ClientSessionHandler clientHandler = null );
}
