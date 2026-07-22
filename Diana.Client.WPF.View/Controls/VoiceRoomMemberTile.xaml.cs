using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diana.Client.WPF.View.Controls;

public partial class VoiceRoomMemberTile : UserControl
{
    public static readonly DependencyProperty UsernameProperty =
        DependencyProperty.Register(nameof(Username), typeof(string), typeof(VoiceRoomMemberTile),
            new PropertyMetadata("User", OnUsernameChanged));

    public static readonly DependencyProperty InitialsProperty =
        DependencyProperty.Register(nameof(Initials), typeof(string), typeof(VoiceRoomMemberTile),
            new PropertyMetadata("?", OnInitialsChanged));

    public static readonly DependencyProperty IsSpeakingProperty =
        DependencyProperty.Register(nameof(IsSpeaking), typeof(bool), typeof(VoiceRoomMemberTile),
            new PropertyMetadata(false, OnIsSpeakingChanged));

    public static readonly DependencyProperty IsMutedProperty =
        DependencyProperty.Register(nameof(IsMuted), typeof(bool), typeof(VoiceRoomMemberTile),
            new PropertyMetadata(false, OnIsMutedChanged));

    public static readonly DependencyProperty ImageBase64Property =
        DependencyProperty.Register(nameof(ImageBase64), typeof(string), typeof(VoiceRoomMemberTile),
            new PropertyMetadata(string.Empty, OnImageBase64Changed));

    public VoiceRoomMemberTile()
    {
        InitializeComponent();
    }

    public string Username
    {
        get => (string)GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }

    public string Initials
    {
        get => (string)GetValue(InitialsProperty);
        set => SetValue(InitialsProperty, value);
    }

    public bool IsSpeaking
    {
        get => (bool)GetValue(IsSpeakingProperty);
        set => SetValue(IsSpeakingProperty, value);
    }

    public bool IsMuted
    {
        get => (bool)GetValue(IsMutedProperty);
        set => SetValue(IsMutedProperty, value);
    }

    public string ImageBase64
    {
        get => (string)GetValue(ImageBase64Property);
        set => SetValue(ImageBase64Property, value);
    }

    private static void OnUsernameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VoiceRoomMemberTile tile)
            tile.UsernameText.Text = e.NewValue?.ToString() ?? "User";
    }

    private static void OnInitialsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VoiceRoomMemberTile tile)
            tile.InitialsText.Text = e.NewValue?.ToString() ?? "?";
    }

    private static void OnIsSpeakingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VoiceRoomMemberTile tile)
        {
            bool speaking = (bool)e.NewValue;
            tile.SpeakingGlow.Opacity = speaking ? 1.0 : 0;
            tile.SpeakingBadge.Visibility = speaking ? Visibility.Visible : Visibility.Collapsed;
            tile.CardBorder.BorderBrush = speaking
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748"));
            tile.CardBorder.BorderThickness = speaking ? new Thickness(2) : new Thickness(1);
        }
    }

    private static void OnIsMutedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VoiceRoomMemberTile tile)
            tile.MuteBadge.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
    }

    private static void OnImageBase64Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VoiceRoomMemberTile tile)
            tile.LoadAvatarFromBase64(e.NewValue?.ToString());
    }

    private void LoadAvatarFromBase64(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            // No image - show initials fallback
            AvatarBrush.ImageSource = null;
            DefaultAvatar.Visibility = Visibility.Visible;
            return;
        }

        try
        {
            var bytes = Convert.FromBase64String(base64);
            using var ms = new MemoryStream(bytes);
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.StreamSource = ms;
            bmp.EndInit();
            bmp.Freeze();

            AvatarBrush.ImageSource = bmp;
            DefaultAvatar.Visibility = Visibility.Collapsed;
        }
        catch
        {
            // Invalid base64 - show initials
            AvatarBrush.ImageSource = null;
            DefaultAvatar.Visibility = Visibility.Visible;
        }
    }
}