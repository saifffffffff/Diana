using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Diana.Application;

internal class VoiceChatService
{


    public readonly UdpClient _udp;

    public VoiceChatService()
    {
        _udp = new UdpClient(9000);
    }

    public async Task Send()
    {

    }



}
