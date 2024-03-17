using System.Configuration;
using System.Data.SQLite;
using System.Windows;
using MinecraftEnchantCalculator.Data.DBModel;
using MinecraftEnchantCalculator.Data.ViewModel;

namespace MinecraftEnchantCalculator.Data.Mapper
{
  public class PrototypeEnchantmentViewMapper
  {
    public static PrototypeEnchantmentViewMapper Instance = new();
    private static Dictionary<int, PrototypeEnchantmentView> _mapper;

    static PrototypeEnchantmentViewMapper()
    {
      _mapper = new Dictionary<int, PrototypeEnchantmentView>();
      string? query = ConfigurationManager.AppSettings["Enchantments"];
      if (query == null) {
        MessageBox.Show("无法获取从数据库获取必要信息，程序即将退出", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown(-1);
      }

      using SQLiteCommand command = new SQLiteCommand(query, DBConnection.Connection);
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