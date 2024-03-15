using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MinecraftEnchantCalculator.Data.ViewModel
{
  public class EnchantmentView : INotifyPropertyChanged
  {
    private bool _isSelected; // 项是否选中
    private int _level;

    public EnchantmentView(PrototypeEnchantmentView prototypeEnchantmentView)
    {
      PrototypeEnchantmentView = prototypeEnchantmentView;
      _level = prototypeEnchantmentView.Enchantment.MaxLevel;
      _isSelected = false;
    }

    public PrototypeEnchantmentView PrototypeEnchantmentView { get; }
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
    public int Level {
      get => _level;
      set {
        if (value < 1 || value > PrototypeEnchantmentView.Enchantment.MaxLevel) {
          return;
        }
        _level = value;
        OnPropertyChanged(nameof(Level));
      }
    }
    public bool IsSelected {
      get => _isSelected;
      set {
        _isSelected = value;
        OnPropertyChanged(nameof(IsSelected));
      }
    }
    #endregion
  }
}