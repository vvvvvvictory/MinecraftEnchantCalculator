using System.Configuration;
using System.Data.SQLite;
using System.Windows;
using MinecraftEnchantCalculator.Data.DBModel;

namespace MinecraftEnchantCalculator.Data.Mapper
{
  public class ApplicableMapper
  {
    public static ApplicableMapper Instance = new();
    private static Dictionary<int, List<int>> _mapper;

    static ApplicableMapper()
    {
      _mapper = new Dictionary<int, List<int>>();
      string? query = ConfigurationManager.AppSettings["Applicable"];
      if (query == null) {
        MessageBox.Show("无法获取从数据库获取必要信息，程序即将退出", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown(-1);
      }
      using SQLiteCommand command = new SQLiteCommand(query, DBConnection.Connection);
      using SQLiteDataReader reader = command.ExecuteReader();
      while (reader.Read()) {
        int itemCode = Convert.ToInt32(reader[nameof(Applicable.ItemCode)]);
        if (!_mapper.ContainsKey(itemCode)) {
          _mapper[itemCode] = new List<int>();
        }
        _mapper[itemCode].Add(Convert.ToInt32(reader[nameof(Applicable.EnchantmentCode)]));
      }
    }

    private ApplicableMapper() { }

    public List<int>? this[int itemCode] => _mapper!.GetValueOrDefault(itemCode, null);
  }
}