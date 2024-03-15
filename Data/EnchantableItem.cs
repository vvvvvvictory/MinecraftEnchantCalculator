using MinecraftEnchantCalculator.Resources.I18n;

namespace MinecraftEnchantCalculator.Data
{
  public class EnchantableItem
  {
    public EnchantableItem(ItemType itemType, int itemId)
    {
      ItemType = itemType;
      ItemId = itemId;
    }

    public ItemType ItemType { get; }           // 物品类型
    public int ItemId { get; }                  // 物品Id，对于工具而言对应数据库Code字段，对于附魔书，暂且不使用
    public int AnvilUseCount { get; set; } = 0; // 铁砧操作数
    public int Cost { get; set; } = 0;          // 附魔达到该物品时过程中花费的经验总数
    public List<EnchantmentViewPerspective> Enchantments { get; private init; } = new();
    public int Value {
      // 物品拥有魔咒的总价值
      get {
        // 由于针对物品只和附魔书进行合并，因此物品不能作为牺牲品，所以当类型为物品时
        // 返回int最大值
        return ItemType == ItemType.ITEM ? int.MaxValue : Enchantments.Sum(evp => evp.Level * evp.BookMultiplier);
      }
    }

    public void AddEnchantment(EnchantmentViewPerspective evp)
    {
      Enchantments.Add(evp);
    }

    public EnchantableItem Clone()
    {
      return new EnchantableItem(ItemType, ItemId) {
        Enchantments = Enchantments.Select(evp => evp.Clone()).ToList()
      };
    }

    /// <summary>
    ///   附魔过程
    /// </summary>
    /// <param name="l">左操作数</param>
    /// <param name="r">右操作数</param>
    /// <returns></returns>
    public static EnchantableItem operator +(EnchantableItem l, EnchantableItem r)
    {
      EnchantableItem target = l.Clone();
      // 计算累计惩罚
      int cost = (1 << l.AnvilUseCount) - 1 + ((1 << r.AnvilUseCount) - 1);

      foreach (EnchantmentViewPerspective rEnc in r.Enchantments) {
        EnchantmentViewPerspective? currentEvp =
          target.Enchantments.FirstOrDefault(targetEnc => rEnc.Code == targetEnc.Code);
        if (currentEvp == null) {
          // 不存在相同魔咒，取牺牲物品上的附魔开销加到附魔成本上。
          cost += rEnc.Level * rEnc.BookMultiplier;
          target.Enchantments.Add(rEnc);
        }
        else {
          // 存在相同魔咒，

          int currentEvpLevel = rEnc.Level == currentEvp.Level
            ? Math.Min(rEnc.Level + 1, rEnc.MaxLevel) // 如果魔咒等级相等，目标物品上的该魔咒等级+1
            : Math.Max(rEnc.Level, currentEvp.Level); // 魔咒等级不等，保留最高级

          cost += currentEvpLevel * currentEvp.BookMultiplier;
          currentEvp.Level = currentEvpLevel;
        }
      }
      target.Cost = cost;
      target.AnvilUseCount = Math.Max(l.AnvilUseCount, r.AnvilUseCount) + 1;
      return target;
    }

    public override string ToString()
    {
      // string itemName = ItemType == ItemType.BOOK
      //   ? Location.Instance[LanguageKey.Default.EnchantedBook] ?? "Enchanted Book"
      //   : Location.Instance[ItemViewMapper.Instance[ItemId].Item.CultureKey] ?? "Enchanted Item";

      string[] roman = { "I", "II", "III", "IV", "V" };
      List<string> ans = Enchantments
        .Select(evp => $"{Location.Instance[evp.CultureKey] ?? evp.CultureKey} {roman[evp.Level - 1]}").ToList();
      return string.Join(" + ", ans);
    }
  }
}