namespace MinecraftEnchantCalculator.Data.DBModel
{
  public class Enchantment
  {
    public Enchantment(int code, string cultureKey, int maxLevel, int itemMultiplier, int bookMultiplier)
    {
      Code = code;
      CultureKey = cultureKey;
      MaxLevel = maxLevel;
      ItemMultiplier = itemMultiplier;
      BookMultiplier = bookMultiplier;
    }

    public int Code { get; }
    public string CultureKey { get; }
    public int MaxLevel { get; }
    public int ItemMultiplier { get; }
    public int BookMultiplier { get; }
  }
}