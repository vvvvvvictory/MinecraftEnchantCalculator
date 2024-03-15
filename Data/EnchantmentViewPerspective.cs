using System.Text.Json.Serialization;
using MinecraftEnchantCalculator.Data.ViewModel;

namespace MinecraftEnchantCalculator.Data
{
  public class EnchantmentViewPerspective
  {
    public EnchantmentViewPerspective(EnchantmentView view)
    {
      Code = view.PrototypeEnchantmentView.Enchantment.Code;
      CultureKey = view.PrototypeEnchantmentView.Enchantment.CultureKey;
      MaxLevel = view.PrototypeEnchantmentView.Enchantment.MaxLevel;
      ItemMultiplier = view.PrototypeEnchantmentView.Enchantment.ItemMultiplier;
      BookMultiplier = view.PrototypeEnchantmentView.Enchantment.BookMultiplier;
      Level = view.Level;
    }

    private EnchantmentViewPerspective() { }

    [JsonConstructor]
    public EnchantmentViewPerspective(int code, string cultureKey, int level, int maxLevel, int itemMultiplier,
      int bookMultiplier)
    {
      Code = code;
      CultureKey = cultureKey;
      Level = level;
      MaxLevel = maxLevel;
      ItemMultiplier = itemMultiplier;
      BookMultiplier = bookMultiplier;
    }

    public int Code { get; set; }
    public string CultureKey { get; set; } = null!;
    public int Level { get; set; }
    public int MaxLevel { get; set; }
    public int ItemMultiplier { get; set; }
    public int BookMultiplier { get; set; }

    public EnchantmentViewPerspective Clone()
    {
      return new EnchantmentViewPerspective {
        Code = Code,
        CultureKey = CultureKey,
        MaxLevel = MaxLevel,
        ItemMultiplier = ItemMultiplier,
        BookMultiplier = BookMultiplier,
        Level = Level
      };
    }
  }
}