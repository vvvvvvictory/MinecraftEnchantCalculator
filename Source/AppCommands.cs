using System.Windows.Input;

namespace MinecraftEnchantCalculator.Source
{
  public class AppCommands
  {
    public static readonly RoutedUICommand CalculateCommand =
      new(nameof(CalculateCommand), nameof(CalculateCommand), typeof(AppCommands));

    public static readonly RoutedUICommand FavoriteCommand =
      new(nameof(FavoriteCommand), nameof(FavoriteCommand), typeof(AppCommands));

    public static readonly RoutedUICommand StopBoringCommand =
      new(nameof(StopBoringCommand), nameof(StopBoringCommand), typeof(AppCommands));
  }
}