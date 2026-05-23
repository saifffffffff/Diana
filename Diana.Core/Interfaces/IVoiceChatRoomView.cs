using System;
using System.Collections.Generic;
using System.Text;

using Diana.Core.Dtos;
namespace Diana.Core.Interfaces;



 
public interface IVoiceChatRoomView
{

    string RoomName { get; set; }
    string RoomKey { get; set; }
    string Username { get; set; }

    void SetMembers(IEnumerable<VoiceChatRoomMember> speakers);    

    bool IsMicrophoneActive { get; set; }

    void CopyRoomKeyToClipboard();
    void CloseView();
    void Mute();
    void UnMute();
    void ShowError(string message);
    
    void VoiceDataRecieved(byte[] data);
    
    event Action <byte[]> OnDataFromMicrophoneReceived;
    event Action OnLeaveRoomPressed;
    event Action OnMicrophoneTogglePressed;
    event Action OnCopyRoomKeyPressed;
    event Action OnSettingsPressed;

}
