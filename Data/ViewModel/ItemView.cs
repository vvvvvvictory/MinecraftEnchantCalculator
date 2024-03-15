using System.ComponentModel;
using System.Runtime.CompilerServices;
using MinecraftEnchantCalculator.Data.DBModel;
using MinecraftEnchantCalculator.Resources.I18n;

namespace MinecraftEnchantCalculator.Data.ViewModel
{
  public class ItemView : INotifyPropertyChanged
  {
    private string _displayName;
    private string _imageSource;

    public ItemView(Item item)
    {
      Item = item;
      _imageSource =
        $"pack://application:,,,/MinecraftEnchantCalculator;component/Resources/images/{Item.CultureKey}.png";
      _displayName = Location.Instance[Item.CultureKey] ?? Item.CultureKey;

      Location.Instance.CultureChanged += () => {
        DisplayName = Location.Instance[Item.CultureKey] ?? Item.CultureKey;
      };
    }

    public Item Item { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

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

    #region UI Binding Properties
    public string DisplayName {
      get => _displayName;
      set {
        _displayName = value;
        OnPropertyChanged(nameof(DisplayName));
      }
    }
    public string ImageSource {
      get => _imageSource;
      set => _imageSource = value ?? throw new ArgumentNullException(nameof(value));
    }
    #endregion
  }
}