using System.Globalization;
using System.Windows.Data;

namespace MinecraftEnchantCalculator.Source
{
  /// <summary>
  ///   转换过程最大值值为5，先不考虑普适情况
  /// </summary>
  public class RomanConverter : IValueConverter
  {
    private readonly Dictionary<string, int> _nums = new() {
      { "I", 1 }, { "II", 2 }, { "III", 3 }, { "IV", 4 }, { "V", 5 }
    };
    private readonly string[] _roman = { "I", "II", "III", "IV", "V" };

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value != null) {
        return _roman[(int)value - 1];
      }
      throw new ArgumentException($"无法转换的参数：{value}");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
      if (value != null) {
        return _nums[(string)value];
      }
      throw new ArgumentException($"无法转换的参数：{value}");
    }
  }
}