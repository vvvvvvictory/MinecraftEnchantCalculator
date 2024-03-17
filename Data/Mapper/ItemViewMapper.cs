using System.Configuration;
using System.Data.SQLite;
using System.Windows;
using MinecraftEnchantCalculator.Data.DBModel;
using MinecraftEnchantCalculator.Data.ViewModel;

namespace MinecraftEnchantCalculator.Data.Mapper
{
  public class ItemViewMapper
  {
    public static ItemViewMapper Instance = new();
    private static Dictionary<int, ItemView> _mapper;

    static ItemViewMapper()
    {
      _mapper = new Dictionary<int, ItemView>();
      string? query = ConfigurationManager.AppSettings["Items"];
      if (query == null) {
        MessageBox.Show("无法获取从数据库获取必要信息，程序即将退出", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown(-1);
      }
      using SQLiteCommand command = new SQLiteCommand(query, DBConnection.Connection);
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