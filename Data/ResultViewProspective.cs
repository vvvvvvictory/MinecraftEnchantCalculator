using System.Text.Json.Serialization;
using MinecraftEnchantCalculator.Data.ViewModel;

namespace MinecraftEnchantCalculator.Data
{
  /// <summary>
  ///   手动收集需要序列化的字段，懒得实现转换器了
  /// </summary>
  public class ResultViewProspective
  {
    public ResultViewProspective(ResultView rv)
    {
      LeftEnchantments = new List<EnchantmentViewPerspective>();
      RightEnchantments = new List<EnchantmentViewPerspective>();
      TargetEnchantments = new List<EnchantmentViewPerspective>();
      Convert(rv);
    }

    [JsonConstructor]
    public ResultViewProspective(string leftImageSource, ItemType leftItemType, int leftItemId,
      List<EnchantmentViewPerspective> leftEnchantments, string rightImageSource, ItemType rightItemType,
      int rightItemId, List<EnchantmentViewPerspective> rightEnchantments, string targetImageSource,
      ItemType targetItemType, int targetItemId, List<EnchantmentViewPerspective> targetEnchantments, int targetCost)
    {
      LeftImageSource = leftImageSource;
      LeftItemType = leftItemType;
      LeftItemId = leftItemId;
      LeftEnchantments = leftEnchantments;
      RightImageSource = rightImageSource;
      RightItemType = rightItemType;
      RightItemId = rightItemId;
      RightEnchantments = rightEnchantments;
      TargetImageSource = targetImageSource;
      TargetItemType = targetItemType;
      TargetItemId = targetItemId;
      TargetEnchantments = targetEnchantments;
      TargetCost = targetCost;
    }

    public string LeftImageSource { get; set; } = null!;
    public ItemType LeftItemType { get; set; }
    public int LeftItemId { get; set; }
    public List<EnchantmentViewPerspective> LeftEnchantments { get; set; }

    public string RightImageSource { get; set; } = null!;
    public ItemType RightItemType { get; set; }
    public int RightItemId { get; set; }
    public List<EnchantmentViewPerspective> RightEnchantments { get; set; }

    public string TargetImageSource { get; set; } = null!;
    public ItemType TargetItemType { get; set; }
    public int TargetItemId { get; set; }
    public List<EnchantmentViewPerspective> TargetEnchantments { get; set; }
    public int TargetCost { get; set; }

    public ResultView ConvertBack()
    {
      EnchantableItem leftEi = new EnchantableItem(LeftItemType, LeftItemId);
      foreach (EnchantmentViewPerspective leftEvp in LeftEnchantments) {
        leftEi.AddEnchantment(leftEvp);
      }

      EnchantableItem rightEi = new EnchantableItem(RightItemType, RightItemId);
      foreach (EnchantmentViewPerspective rightEvp in RightEnchantments) {
        rightEi.AddEnchantment(rightEvp);
      }

      EnchantableItem targetEi = new EnchantableItem(TargetItemType, TargetItemId);
      foreach (EnchantmentViewPerspective targetEvp in TargetEnchantments) {
        targetEi.AddEnchantment(targetEvp);
        targetEi.Cost = TargetCost;
      }
      return new ResultView(new EnchantableItemView(leftEi),
        new EnchantableItemView(rightEi),
        new EnchantableItemView(targetEi));
    }

    public void Convert(ResultView rv)
    {
      LeftImageSource = rv.Left.ImageSource;
      LeftItemType = rv.Left.EnchantableItem.ItemType;
      LeftItemId = rv.Left.EnchantableItem.ItemId;
      LeftEnchantments = rv.Left.EnchantableItem.Enchantments;

      RightImageSource = rv.Right.ImageSource;
      RightItemType = rv.Right.EnchantableItem.ItemType;
      RightItemId = rv.Right.EnchantableItem.ItemId;
      RightEnchantments = rv.Right.EnchantableItem.Enchantments;

      TargetImageSource = rv.Target.ImageSource;
      TargetItemType = rv.Target.EnchantableItem.ItemType;
      TargetItemId = rv.Target.EnchantableItem.ItemId;
      TargetCost = rv.Target.EnchantableItem.Cost;
      TargetEnchantments = rv.Target.EnchantableItem.Enchantments;
    }
  }
}