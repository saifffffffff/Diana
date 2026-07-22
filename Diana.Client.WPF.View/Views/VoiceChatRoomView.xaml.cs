using Diana.Core.Dtos;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using Diana.Core.Abstractions.Views;

namespace Diana.Client.WPF.View.Views;

/// <summary>
/// WPF view model wrapper around VoiceChatRoomMember for data binding display properties like Initials
/// </summary>
public class VoiceChatRoomMemberDisplay
{
    public string Username { get; set; }
    public string ImageBase64 { get; set; }
    public bool IsMuted { get; set; }
    public bool IsSpeaking { get; set; }
    public string Initials { get; set; }

    public static VoiceChatRoomMemberDisplay FromDto(VoiceChatRoomMember dto)
    {
        var initials = string.IsNullOrWhiteSpace(dto.Username) ? "?" 
            : string.Concat(dto.Username.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => s[0])).ToUpper();
        if (initials.Length > 2) initials = initials[..2];
        
        return new VoiceChatRoomMemberDisplay
        {
            Username = dto.Username,
            ImageBase64 = dto.ImageBase64,
            IsMuted = dto.IsMuted,
            IsSpeaking = dto.IsSpeaking,
            Initials = initials
        };
    }
}

/// <summary>
/// Interaction logic for VoiceChatRoomView.xaml
/// </summary>
public partial class VoiceChatRoomView : Window, IVoiceChatRoomView 
{
    BufferedWaveProvider bufferOut;
    WaveInEvent waveIn;
    WaveOut waveOut;
    int waveRate = 48_000; // 48 KHz

    private readonly ObservableCollection<VoiceChatRoomMemberDisplay> _members = new();

    public VoiceChatRoomView(string roomName, string roomKey, string username)
    {
        RoomName = roomName;
        RoomKey = roomKey;
        Username = username;

        waveOut = new WaveOut();
        bufferOut = new BufferedWaveProvider(new WaveFormat(waveRate, 1));
        bufferOut.DiscardOnBufferOverflow = true;
        waveOut.Init(bufferOut);
        waveOut.Play();
        waveIn = new WaveInEvent();
        waveIn.BufferMilliseconds = 20; // each 20 ms the buffer fires
        InitializeComponent();

        RoomNameText.Text = roomName ?? "Voice Room";
        MembersContainer.ItemsSource = _members;
        SelfUsernameText.Text = username ?? "Me";
        UpdateMemberCount();
    }

    public string RoomName { get; set; }
    public string RoomKey { get; set; }
    public string Username { get; set; }
    public bool IsMicrophoneActive { get; set; } = true;

    public event Action<byte[]> OnDataFromMicrophoneReceived;
    public event Action OnLeaveRoomPressed;
    public event Action OnMicrophoneTogglePressed;
    public event Action OnCopyRoomKeyPressed;
    public event Action OnSettingsPressed;

    public void CloseView()
    {
        ExecuteOnUi(() => this.Close());
    }

    public void CopyRoomKeyToClipboard()
    {
        Clipboard.SetText(RoomKey ?? string.Empty);
        
    }

    public void SetMembers(IEnumerable<VoiceChatRoomMember> speakers)
    {
        ExecuteOnUi(() =>
        {
            _members.Clear();
            foreach (var s in speakers)
            {
                _members.Add(VoiceChatRoomMemberDisplay.FromDto(s));
            }
            UpdateMemberCount();
        });
    }

    public void UpdateSingleMember(string username, Action<VoiceChatRoomMemberDisplay> updateAction)
    {
        ExecuteOnUi(() =>
        {
            var member = _members.FirstOrDefault(m => m.Username == username);
            if (member != null)
            {
                updateAction(member);
                // Force the UI to refresh by replacing the item
                var index = _members.IndexOf(member);
                _members.RemoveAt(index);
                _members.Insert(index, member);
            }
        });
    }

    public void ShowError(string message)
    {
        ExecuteOnUi(() => MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
    }

    public void VoiceDataRecieved(byte[] receivedBytes)
    {
        int maxBufferBytes = 48_000 * 2 * 1 / 5; // 19200 bytes per 200ms

        if (bufferOut.BufferedBytes > maxBufferBytes)
        {
            bufferOut.ClearBuffer();
        }

        bufferOut.AddSamples(receivedBytes, 0, receivedBytes.Length);
    }

    public void Mute()
    {
        IsMicrophoneActive = false;
        UpdateMicButtonUI();
    }

    public void UnMute()
    {
        IsMicrophoneActive = true;
        UpdateMicButtonUI();
    }

    private void UpdateMicButtonUI()
    {
        ExecuteOnUi(() =>
        {
            if (IsMicrophoneActive)
            {
                MicIcon.Text = "🎤";
                MicLabel.Text = "Mute";
                MicButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#21283A"));
            }
            else
            {
                MicIcon.Text = "🔇";
                MicLabel.Text = "Unmute";
                MicButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D1B1B"));
            }
        });
    }

    private void UpdateMemberCount()
    {
        MemberCountText.Text = $"{_members.Count} member{(_members.Count != 1 ? "s" : "")}";
    }

    private void ExecuteOnUi(Action action)
    {
        if (Dispatcher.CheckAccess()) action();
        else Dispatcher.Invoke(action);
    }

    // ─────────────────────────── Event Handlers ───────────────────────────

    private void Window_Closed(object sender, EventArgs e)
    {
        OnLeaveRoomPressed?.Invoke();
        waveIn.Dispose();
        waveOut.Dispose();

    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        waveIn.WaveFormat = new WaveFormat(waveRate, 1);

        waveIn.DataAvailable += (s, _e) =>
        {
            if (!IsMicrophoneActive)
                return;

            byte[] audioData = new byte[_e.BytesRecorded];
            Array.Copy(_e.Buffer, audioData, _e.BytesRecorded);
            OnDataFromMicrophoneReceived?.Invoke(audioData);
        };

        waveIn.StartRecording();
    }

    private void MicButton_Click(object sender, RoutedEventArgs e)
    {
        OnMicrophoneTogglePressed?.Invoke();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        OnSettingsPressed?.Invoke();
    }

    private void LeaveButton_Click(object sender, RoutedEventArgs e)
    {
        OnLeaveRoomPressed?.Invoke();
        waveIn.Dispose();
        waveOut.Dispose();
    }

    private void CopyKeyButton_Click(object sender, RoutedEventArgs e)
    {
        OnCopyRoomKeyPressed?.Invoke();
    }

    // ──────────── Custom Title Bar ────────────

    private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
            this.DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = this.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
