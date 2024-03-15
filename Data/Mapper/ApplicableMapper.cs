using System.Data.SQLite;
using MinecraftEnchantCalculator.Data.DBModel;
using MinecraftEnchantCalculator.Resources.Sqlite;

namespace MinecraftEnchantCalculator.Data.Mapper
{
  public class ApplicableMapper
  {
    public static ApplicableMapper Instance = new();
    private static readonly string _query = string.Format(SqliteConfig.Default.Applicable, nameof(Applicable.ItemCode),
      nameof(Applicable.EnchantmentCode));
    private static Dictionary<int, List<int>> _mapper;

    static ApplicableMapper()
    {
      _mapper = new Dictionary<int, List<int>>();
      using SQLiteCommand command = new SQLiteCommand(_query, DBConnection.Connection);
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