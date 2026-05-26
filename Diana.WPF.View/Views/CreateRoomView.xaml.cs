using Diana.Core;
using Diana.Core.Interfaces;
using Diana.Presenter;
using System.Windows;
using System.Windows.Media;

namespace Diana.WPF.View.Views;

public partial class CreateRoomView : Window, ICreateRoomView
{
    public CreateRoomView(string username, string ImageBase64 = null )
    {
        InitializeComponent();
        CreateButton.Click += (s, e) => OnCreatePressed?.Invoke();
        this.Username = username ?? string.Empty;
        this.ImageBase64 = ImageBase64;
        // Default to Voice room
        SelectVoiceRoom();
    }

    public RoomType SelectedRoomType { get; set; } = RoomType.Voice;

    public string RoomName
    {
        get
        {
            if (RoomNameBox.Dispatcher.CheckAccess())
                return RoomNameBox.Text;
            return (string)RoomNameBox.Dispatcher.Invoke(() => RoomNameBox.Text);
        }
        set => ExecuteOnUi(() => RoomNameBox.Text = value);
    }
    public string RoomKey
    {
        get
        {
            if (RoomKeyBox.Dispatcher.CheckAccess())
                return RoomKeyBox.Text;
            return (string)RoomKeyBox.Dispatcher.Invoke(() => RoomKeyBox.Text);
        }
        set => ExecuteOnUi(() => RoomKeyBox.Text = value);
    }
    public string Username { get; set; }
    public bool IsRoomCreated { get; set; }
    public string ImageBase64 { get; set ; }

    public event Action OnCreatePressed;
    public event Action OnViewClosed;

    public void ShowError(string message) => ExecuteOnUi(() => MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
    public void CloseView() => ExecuteOnUi(() => this.Close());
    public void HideView() => ExecuteOnUi(() => this.Hide());
    public void ShowView() => ExecuteOnUi(() => this.Show());

    private void Window_Closed(object sender, System.EventArgs e)
    {
        OnViewClosed?.Invoke();
    }

    private void ExecuteOnUi(Action action)
    {
        if (Dispatcher.CheckAccess()) action();
        else Dispatcher.Invoke(action);
    }

   

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    // ──────────── Custom Title Bar ────────────

    private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1)
            this.DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void SelectVoiceRoom()
    {
        SelectedRoomType = RoomType.Voice;
        VoiceTypeBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6366F1"));
        VoiceTypeBorder.BorderThickness = new Thickness(2);
        VoiceRadioDot.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6366F1"));
        ChatTypeBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748"));
        ChatTypeBorder.BorderThickness = new Thickness(1);
        ChatRadioDot.Fill = Brushes.Transparent;
    }

    private void SelectChatRoom()
    {
        SelectedRoomType = RoomType.Chat;
        ChatTypeBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6366F1"));
        ChatTypeBorder.BorderThickness = new Thickness(2);
        ChatRadioDot.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6366F1"));
        VoiceTypeBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748"));
        VoiceTypeBorder.BorderThickness = new Thickness(1);
        VoiceRadioDot.Fill = Brushes.Transparent;
    }

    private void VoiceType_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        SelectVoiceRoom();
    }

    private void ChatType_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        SelectChatRoom();
    }
}
