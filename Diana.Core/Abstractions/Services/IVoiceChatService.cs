using Diana.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Diana.Core.Abstractions.Services;

public interface IVoiceChatService
{
    
    void Connect(string ipAddress , int port);
    
    void Disconnect();

    Task SendAsync(VoiceChatMessage msg);

    bool IsConnected { get; }
    IPEndPoint EndPoint { get; }

    void Receive();
    
    public event Action<byte[]>? OnVoiceDataRecieved;

}
