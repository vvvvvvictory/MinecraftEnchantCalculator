using System.Configuration;
using System.Data.SQLite;
using System.Windows;
using MinecraftEnchantCalculator.Data.DBModel;

namespace MinecraftEnchantCalculator.Data.Mapper
{
  public class ConflictsMapper
  {
    public static ConflictsMapper Instance = new();

    private static Dictionary<int, List<int>> _mapper;

    static ConflictsMapper()
    {
      _mapper = new Dictionary<int, List<int>>();
      string? query = ConfigurationManager.AppSettings["Conflicts"];
      if (query == null) {
        MessageBox.Show("无法获取从数据库获取必要信息，程序即将退出", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown(-1);
      }

      using SQLiteCommand command = new SQLiteCommand(query, DBConnection.Connection);
      using SQLiteDataReader reader = command.ExecuteReader();
      while (reader.Read()) {
        int enchantmentCode_1 = Convert.ToInt32(reader[nameof(Conflict.EnchantmentCode_1)]);
        int enchantmentCode_2 = Convert.ToInt32(reader[nameof(Conflict.EnchantmentCode_2)]);
        if (!_mapper.ContainsKey(enchantmentCode_1)) {
          _mapper[enchantmentCode_1] = new List<int>();
        }
        if (!_mapper.ContainsKey(enchantmentCode_2)) {
          _mapper[enchantmentCode_2] = new List<int>();
        }
        _mapper[enchantmentCode_1].Add(enchantmentCode_2);
        _mapper[enchantmentCode_2].Add(enchantmentCode_1);
      }
    }

    private ConflictsMapper() { }

    public List<int>? this[int code] => _mapper!.GetValueOrDefault(code, null);
  }
}