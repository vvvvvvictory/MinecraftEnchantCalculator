namespace MinecraftEnchantCalculator.Data.ViewModel
{
  public class ResultView
  {
    public ResultView(EnchantableItemView left, EnchantableItemView right, EnchantableItemView target)
    {
      Left = left;
      Right = right;
      Target = target;
    }

    public EnchantableItemView Left { get; set; }
    public EnchantableItemView Right { get; set; }
    public EnchantableItemView Target { get; set; }
  }
}