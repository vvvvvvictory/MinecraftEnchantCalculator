using System.Windows.Input;

namespace MinecraftEnchantCalculator
{
  public class AppCommands
  {
    public static readonly RoutedUICommand CalculateCommand =
      new(nameof(CalculateCommand), nameof(CalculateCommand), typeof(AppCommands));

    public static readonly RoutedUICommand FavoriteCommand =
      new(nameof(FavoriteCommand), nameof(FavoriteCommand), typeof(AppCommands));
  }
}