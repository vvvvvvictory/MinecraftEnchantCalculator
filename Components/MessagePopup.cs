using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MinecraftEnchantCalculator.Components
{
  [TemplatePart(Name = "X_PART_Icon", Type = typeof(Image))]
  [TemplatePart(Name = "X_PART_Border", Type = typeof(Border))]
  [TemplatePart(Name = "X_PART_Message", Type = typeof(TextBlock))]
  public class MessagePopup : Control, INotifyPropertyChanged
  {
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon),
      typeof(ImageSource),
      typeof(MessagePopup), new PropertyMetadata(null));

    public static readonly DependencyProperty MessageTypeProperty = DependencyProperty.Register(nameof(MessageType),
      typeof(MessageType), typeof(MessagePopup), new PropertyMetadata(MessageType.INFO));

    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message),
      typeof(string), typeof(MessagePopup), new PropertyMetadata(string.Empty));

    static MessagePopup()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(MessagePopup),
        new FrameworkPropertyMetadata(typeof(MessagePopup)));
    }

    public ImageSource Icon {
      get => (ImageSource)GetValue(IconProperty);
      set {
        SetValue(IconProperty, value);
        OnPropertyChanged(nameof(Icon));
      }
    }

    public MessageType MessageType {
      get => (MessageType)GetValue(MessageTypeProperty);
      set {
        SetValue(MessageTypeProperty, value);
        OnPropertyChanged(nameof(MessageType));
      }
    }

    public string Message {
      get => (string)GetValue(MessageProperty);
      set {
        SetValue(MessageProperty, value);
        OnPropertyChanged(nameof(Message));
      }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Toggle(MessageType type, string msg)
    {
      MessageType = type;
      Message = msg;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
    }
  }
}