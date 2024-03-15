using System.Data.SQLite;
using MinecraftEnchantCalculator.Data.DBModel;
using MinecraftEnchantCalculator.Data.ViewModel;
using MinecraftEnchantCalculator.Resources.Sqlite;

namespace MinecraftEnchantCalculator.Data.Mapper
{
  public class PrototypeEnchantmentViewMapper
  {
    public static PrototypeEnchantmentViewMapper Instance = new();
    private static readonly string _query = string.Format(SqliteConfig.Default.Enchantments,
      nameof(Enchantment.Code), nameof(Enchantment.CultureKey), nameof(Enchantment.MaxLevel),
      nameof(Enchantment.ItemMultiplier), nameof(Enchantment.BookMultiplier));
    private static Dictionary<int, PrototypeEnchantmentView> _mapper;

    static PrototypeEnchantmentViewMapper()
    {
      _mapper = new Dictionary<int, PrototypeEnchantmentView>();

      using SQLiteCommand command = new SQLiteCommand(_query, DBConnection.Connection);
      using SQLiteDataReader reader = command.ExecuteReader();
      while (reader.Read()) {
        int code = Convert.ToInt32(reader[nameof(Enchantment.Code)]);
        _mapper.Add(code, new PrototypeEnchantmentView(new Enchantment(code,
          Convert.ToString(reader[nameof(Enchantment.CultureKey)])!,
          Convert.ToInt32(reader[nameof(Enchantment.MaxLevel)]),
          Convert.ToInt32(reader[nameof(Enchantment.ItemMultiplier)]),
          Convert.ToInt32(reader[nameof(Enchantment.BookMultiplier)]))));
      }
    }

    private PrototypeEnchantmentViewMapper() { }

    public PrototypeEnchantmentView this[int code] => _mapper[code];

    /// <summary>
    ///   返回代码为code的原型对象的可变Level对象
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public EnchantmentView View(int code)
    {
      return new EnchantmentView(this[code]);
    }

    public List<EnchantmentView> ViewModels()
    {
      List<EnchantmentView> ans = new List<EnchantmentView>();
      foreach (PrototypeEnchantmentView pev in _mapper.Values) {
        ans.Add(new EnchantmentView(pev));
      }
      return ans;
    }
  }
}