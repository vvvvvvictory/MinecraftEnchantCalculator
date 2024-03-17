using System.Globalization;
using System.Windows.Data;
using MinecraftEnchantCalculator.Resources.I18n;
using MinecraftEnchantCalculator.Resources.Settings;

namespace MinecraftEnchantCalculator.Source
{
  public class FeasibleConverter : IValueConverter
  {
    // private readonly Dictionary<string, bool> _d = new() {
    //   { LanguageKey.Default.FeasibleYes, true }, { LanguageKey.Default.FeasibleNo, false }
    // };

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value != null) {
        bool b = (bool)value;
        return (b
          ? Location.Instance[LanguageKey.Default.FeasibleYes]!
          : Location.Instance[LanguageKey.Default.FeasibleNo])!;
      }
      throw new ArgumentException($"无法转换的参数：{value}");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}