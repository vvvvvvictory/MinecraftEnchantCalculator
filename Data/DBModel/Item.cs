namespace MinecraftEnchantCalculator.Data.DBModel
{
  public class Item
  {
    public Item(int code, string cultureKey)
    {
      Code = code;
      CultureKey = cultureKey;
    }

    public int Code { get; }
    public string CultureKey { get; }
  }
}