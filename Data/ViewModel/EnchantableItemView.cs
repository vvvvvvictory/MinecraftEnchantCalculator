using System.ComponentModel;
using System.Runtime.CompilerServices;
using MinecraftEnchantCalculator.Data.Mapper;
using MinecraftEnchantCalculator.Resources.I18n;
using MinecraftEnchantCalculator.Resources.Settings;

namespace MinecraftEnchantCalculator.Data.ViewModel
{
  public class EnchantableItemView : INotifyPropertyChanged
  {
    private string _displayName;

    public EnchantableItemView(EnchantableItem EnchantableItem)
    {
      this.EnchantableItem = EnchantableItem;
      string cultureKey = EnchantableItem.ItemType == ItemType.BOOK
        ? LanguageKey.Default.EnchantedBook
        : ItemViewMapper.Instance[EnchantableItem.ItemId].Item.CultureKey;
      _displayName = Location.Instance[cultureKey] ?? cultureKey;
      ImageSource = EnchantableItem.ItemType == ItemType.BOOK
        ? $"pack://application:,,,/MinecraftEnchantCalculator;component/Resources/images/{LanguageKey.Default.EnchantedBook}.png"
        : $"pack://application:,,,/MinecraftEnchantCalculator;component/Resources/images/{cultureKey}.png";

      Location.Instance.CultureChanged += () => {
        DisplayName = Location.Instance[cultureKey] ?? cultureKey;
        OnPropertyChanged(nameof(Description));
      };
    }

    public EnchantableItem EnchantableItem { get; }
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

    #region UI Binging Properties
    public string DisplayName {
      get => _displayName;
      set {
        _displayName = value;
        OnPropertyChanged(nameof(DisplayName));
      }
    }
    public string Description => EnchantableItem.ToString();
    public string ImageSource { get; }
    #endregion
  }
}