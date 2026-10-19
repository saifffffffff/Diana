using Diana.Core.Abstractions.Networking;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Serialization;
using Diana.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Diana.Server.Networking;

class UdpVoiceStreamRelay(ISerializer serializer, IRoomsManager roomManager) : IVoiceStreamRelay
{
    public async Task RunAsync()
    {
        // TODO: move port to config file
        var udp = new UdpClient(5000);


        while (true)
        {

            var result = await udp.ReceiveAsync();
            string msgStr = Encoding.UTF8.GetString(result.Buffer);
            var msg = serializer.Deserialize<VoiceChatMessage>(msgStr);

            if (msg is null) continue;

            var room = roomManager.GetRoomByKey(msg.RoomKey);

            Console.WriteLine(string.Join( ' ' , msg.VoiceData));


            foreach (var member in room.Members)
            {

                if (member.Username == msg.SenderUsername) continue;

                if (member.EndPoint is not null)
                {
                    await udp.SendAsync(msg.VoiceData, msg.VoiceData.Length, member.EndPoint);
                }
            }

        }
    }
}
