using Diana.Core.Abstractions.Rooms;
using Diana.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
namespace Diana.Core.Abstractions.Sessions;

public interface ISession : IAsyncDisposable
{
    string? Username { get; set; }
    string? ImageBase64 { get; set; }
    bool IsMuted { get; set; }
    bool IsSpeaking { get; set; }
    IPEndPoint? EndPoint { get; set; }
    IChatRoom? CurrentRoom { get; set; } // type to make an abstraction for it 

    Task RecieveAsync();

    Task SendAsync(ServerResponse message);

    Task HandleRequestAsync(ClientRequest request);
}
