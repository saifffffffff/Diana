using System;
using System.Collections.Generic;
using System.Text;

namespace Diana.Core.Dtos;


public record VoiceChatRoomMember (
    string Username,
    string ImageBase64,
    bool IsMuted,
    bool IsSpeaking
);