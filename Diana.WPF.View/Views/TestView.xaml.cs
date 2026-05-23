using Diana.Core.Dtos;
using Diana.Core.Interfaces;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
  

namespace Diana.WPF.View.Views
{
    /// <summary>
    /// Interaction logic for TestView.xaml
    /// </summary>
    public partial class TestView : Window, IVoiceChatRoomView
    {

        BufferedWaveProvider bufferOut;
        WaveInEvent waveIn;
        WaveOut waveOut;
        int waveRate = 48_000; // 48 KHz 
        public TestView(string roomName, string roomKey, string username)
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
            waveIn.BufferMilliseconds = 20; // each 20 ms the b
            InitializeComponent();
        }


        public string RoomName { get; set; }
        public string RoomKey { get; set; }
        public string Username { get; set; }
        public bool IsMicrophoneActive { get; set; }

        public event Action<byte[]> OnDataFromMicrophoneReceived;
        public event Action OnLeaveRoomPressed;
        public event Action OnMicrophoneTogglePressed;
        public event Action OnCopyRoomKeyPressed;
        public event Action OnSettingsPressed;

        public void CloseView()
        {
            this.Close();
        }

        public void CopyRoomKeyToClipboard()
        {
            throw new NotImplementedException();
        }

        public void SetMembers(IEnumerable<VoiceChatRoomMember> speakers)
        {
            throw new NotImplementedException();
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }



        public void VoiceDataRecieved(byte[] receivedBytes)
        {
            int maxBufferBytes = 48_000 * 2 * 1 / 5; // 19200 bytes per 200ms 

            if (bufferOut.BufferedBytes > maxBufferBytes)
            {
                bufferOut.ClearBuffer(); // ✓ throw away stale audio, start fresh
            }


            bufferOut.AddSamples(receivedBytes, 0, receivedBytes.Length);

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

        private void Button_Click(object sender, RoutedEventArgs e) {

            
            OnMicrophoneTogglePressed?.Invoke();
            //if(IsMicrophoneActive)
                


        }



        public void Mute() => IsMicrophoneActive = false;
        

        public void UnMute() => IsMicrophoneActive = true;

        private void Window_Closed(object sender, EventArgs e)
        {
            OnLeaveRoomPressed?.Invoke();
        }
    }
}
