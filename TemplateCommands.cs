using System.Windows.Input;

namespace MinecraftEnchantCalculator
{
  public static class TemplateCommands
  {
    public static readonly RoutedUICommand EnchantLevelIncreasing = new(nameof(EnchantLevelIncreasing),
      nameof(EnchantLevelIncreasing), typeof(TemplateCommands));

    public static readonly RoutedUICommand EnchantLevelDecreasing = new(nameof(EnchantLevelDecreasing),
      nameof(EnchantLevelDecreasing), typeof(TemplateCommands));

    public static readonly RoutedUICommand FavoriteViewsDelete = new(nameof(FavoriteViewsDelete),
      nameof(FavoriteViewsDelete), typeof(TemplateCommands));
  }
}