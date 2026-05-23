using Diana.Application;
using Diana.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diana.WPF.View;

internal static class DIContainer
{
    public static IChatService chatService = new ChatService();
    public static IVoiceChatService voiceChatService = new VoiceChatService();
    public static IAudioCodec audioCodec = new OpusAudioCodec();
}
