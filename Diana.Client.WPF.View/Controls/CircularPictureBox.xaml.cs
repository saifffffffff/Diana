using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Diana.Client.WPF.View.Controls;

public partial class CircularPictureBox : UserControl
{
    /// <summary>Fires when the user selects a new image. Returns the base64-encoded string.</summary>
    public event Action<string> OnImageSelected;

    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(CircularPictureBox),
            new PropertyMetadata(null, OnImageSourceChanged));

    public static readonly DependencyProperty UsernameProperty =
        DependencyProperty.Register(nameof(Username), typeof(string), typeof(CircularPictureBox),
            new PropertyMetadata("", OnUsernameChanged));

    public static readonly DependencyProperty ImageBase64Property =
        DependencyProperty.Register(nameof(ImageBase64), typeof(string), typeof(CircularPictureBox),
            new PropertyMetadata(string.Empty));

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public string Username
    {
        get => (string)GetValue(UsernameProperty);
        set => SetValue(UsernameProperty, value);
    }

    /// <summary>The base64-encoded image string. Set this to load an image from base64.</summary>
    public string ImageBase64
    {
        get
        {
            if (Dispatcher.CheckAccess())
                return (string)GetValue(ImageBase64Property);
            return Dispatcher.Invoke(() => (string)GetValue(ImageBase64Property));
        }
        set => SetValue(ImageBase64Property, value);
    }

    public CircularPictureBox()
    {
        InitializeComponent();
        UpdateInitials();
    }

    private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CircularPictureBox)d;
        if (e.NewValue is ImageSource img)
        {
            control.PictureImageBrush.ImageSource = img;
            control.InitialsBorder.Visibility = Visibility.Collapsed;
            control.PictureEllipse.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6366F1"));
        }
        else
        {
            control.PictureImageBrush.ImageSource = null;
            control.InitialsBorder.Visibility = Visibility.Visible;
            control.PictureEllipse.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D3748"));
        }
    }

    private static void OnUsernameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((CircularPictureBox)d).UpdateInitials();
    }

    private void UpdateInitials()
    {
        var name = Username?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            InitialsText.Text = "?";
            return;
        }
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            InitialsText.Text = $"{parts[0][0]}{parts[1][0]}".ToUpper();
        else
            InitialsText.Text = parts[0][0].ToString().ToUpper();
    }

    private void OpenImageDialog()
    {
        Dispatcher.Invoke(() =>
        {
            var dlg = new OpenFileDialog
            {
                Title = "Select Profile Picture",
                Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files (*.*)|*.*",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() == true)
                LoadImageFromFile(dlg.FileName);
        });
    }

    private void LoadImageFromFile(string filePath)
    {
        try
        {
            // Load image into a BitmapImage
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new Uri(filePath, UriKind.Absolute);
            bmp.EndInit();
            bmp.Freeze();

            ImageSource = bmp;

            // Convert to base64
            var extension = Path.GetExtension(filePath).ToLower();
            var format = extension switch
            {
                ".png" => ImgCodec.Png,
                ".jpg" or ".jpeg" => ImgCodec.Jpeg,
                ".bmp" => ImgCodec.Bmp,
                ".gif" => ImgCodec.Gif,
                _ => ImgCodec.Png
            };

            var base64 = BitmapImageToBase64(bmp, format);
            ImageBase64 = base64;
            OnImageSelected?.Invoke(base64);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>Loads an image from a base64 string (e.g., from the server) and displays it.</summary>
    public void LoadFromBase64(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64)) return;

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

            ImageSource = bmp;
            ImageBase64 = base64;
        }
        catch { /* ignore invalid base64 */ }
    }

    private static string BitmapImageToBase64(BitmapImage image, string encoderFormat)
    {
        System.Windows.Media.Imaging.BitmapEncoder encoder = encoderFormat switch
        {
            ImgCodec.Jpeg => new JpegBitmapEncoder { QualityLevel = 80 },
            ImgCodec.Png => new PngBitmapEncoder(),
            ImgCodec.Bmp => new BmpBitmapEncoder(),
            ImgCodec.Gif => new GifBitmapEncoder(),
            _ => new PngBitmapEncoder()
        };

        encoder.Frames.Add(BitmapFrame.Create(image));

        using var ms = new MemoryStream();
        encoder.Save(ms);
        return Convert.ToBase64String(ms.ToArray());
    }

    // ──────────────── Event Handlers ────────────────

    private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        AvatarOverlay.Opacity = 0.4;
        GlowRing.Opacity = 1.0;
    }

    private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
        AvatarOverlay.Opacity = 0;
        GlowRing.Opacity = 0;
    }

    private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        OpenImageDialog();
    }

    /// <summary>
    /// Constants for BitmapEncoder format strings (avoids magic strings)
    /// </summary>
    public static class ImgCodec
    {
        public const string Jpeg = "jpeg";
        public const string Png = "png";
        public const string Bmp = "bmp";
        public const string Gif = "gif";
    }
}