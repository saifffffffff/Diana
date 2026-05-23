
namespace  Diana.Core.Dtos;

public class VoiceChatMessage
{
    public string SenderUsername { get; set; }
    public string RoomKey { get; set; }

    public byte[] VoiceData { get; set; }

}
