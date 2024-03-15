namespace MinecraftEnchantCalculator.Data.DBModel
{
  public class Applicable
  {
    public Applicable(int itemCode, int enchantmentCode)
    {
      ItemCode = itemCode;
      EnchantmentCode = enchantmentCode;
    }

    public int ItemCode { get; }
    public int EnchantmentCode { get; }
  }
}