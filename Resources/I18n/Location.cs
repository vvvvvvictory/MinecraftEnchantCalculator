using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace MinecraftEnchantCalculator.Resources.I18n
{
  public class Location : INotifyPropertyChanged
  {
    private static readonly Lazy<Location> lazy = new(() => new Location());
    private readonly ResourceManager _resourceManager =
      new($"{typeof(Location).Namespace}.Culture", typeof(Location).Assembly);
    public static Location Instance => lazy.Value;

    public string? this[string key] {
      get {
        if (key == null) throw new ArgumentNullException(nameof(key));
        return _resourceManager.GetString(key);
      }
    }

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

    public event Action? CultureChanged;

    public void ChangeLocation(CultureInfo culture)
    {
      CultureInfo.CurrentCulture = culture;
      CultureInfo.CurrentUICulture = culture;
      PropertyChanged?.Invoke(this,
        new PropertyChangedEventArgs(Binding.IndexerName));
      CultureChanged?.Invoke();
    }
  }
}