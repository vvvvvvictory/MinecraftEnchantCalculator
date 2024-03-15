using System.ComponentModel;
using System.Runtime.CompilerServices;
using MinecraftEnchantCalculator.Data.DBModel;
using MinecraftEnchantCalculator.Resources.I18n;

namespace MinecraftEnchantCalculator.Data.ViewModel
{
  public class PrototypeEnchantmentView : INotifyPropertyChanged
  {
    private string _displayName; // 显示名字
    private bool _isEnabled;     // 项是否可用

    public PrototypeEnchantmentView(Enchantment enchantment)
    {
      Enchantment = enchantment;
      _displayName = Location.Instance[Enchantment.CultureKey] ?? Enchantment.CultureKey;
      _isEnabled = true;

      Location.Instance.CultureChanged += () => {
        DisplayName = Location.Instance[Enchantment.CultureKey] ?? Enchantment.CultureKey;
      };
    }

    public Enchantment Enchantment { get; }
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

    #region UI Bandings Properties
    public bool IsEnabled {
      get => _isEnabled;
      set {
        _isEnabled = value;
        OnPropertyChanged(nameof(IsEnabled));
      }
    }
    public string DisplayName {
      get => _displayName;
      set {
        _displayName = value;
        OnPropertyChanged(nameof(DisplayName));
      }
    }
    #endregion
  }
}