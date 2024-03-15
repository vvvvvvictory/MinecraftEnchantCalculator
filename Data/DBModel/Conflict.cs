namespace MinecraftEnchantCalculator.Data.DBModel
{
  public class Conflict
  {
    public Conflict(int enchantmentCode1, int enchantmentCode2)
    {
      EnchantmentCode_1 = enchantmentCode1;
      EnchantmentCode_2 = enchantmentCode2;
    }

    public int EnchantmentCode_1 { get; }
    public int EnchantmentCode_2 { get; }
  }
}