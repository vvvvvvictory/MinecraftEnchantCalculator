using System.Data.SQLite;
using MinecraftEnchantCalculator.Data.DBModel;
using MinecraftEnchantCalculator.Data.ViewModel;
using MinecraftEnchantCalculator.Resources.Sqlite;

namespace MinecraftEnchantCalculator.Data.Mapper
{
  public class ItemViewMapper
  {
    public static ItemViewMapper Instance = new();
    private static readonly string _query =
      string.Format(SqliteConfig.Default.Items, nameof(Item.Code), nameof(Item.CultureKey));
    private static Dictionary<int, ItemView> _mapper;

    static ItemViewMapper()
    {
      _mapper = new Dictionary<int, ItemView>();

      using SQLiteCommand command = new SQLiteCommand(_query, DBConnection.Connection);
      using SQLiteDataReader reader = command.ExecuteReader();
      while (reader.Read()) {
        int code = Convert.ToInt32(reader[nameof(Item.Code)]);
        _mapper.Add(code, new ItemView(new Item(code,
          Convert.ToString(reader[nameof(Item.CultureKey)])!)));
      }
    }

    private ItemViewMapper() { }

    public ItemView this[int itemCode] => _mapper[itemCode];

    public List<ItemView> ViewModels()
    {
      List<ItemView> ans = new List<ItemView>();
      foreach (ItemView iv in _mapper.Values) {
        ans.Add(iv);
      }
      return ans;
    }
  }
}