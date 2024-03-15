using System.Data.SQLite;
using MinecraftEnchantCalculator.Data.DBModel;
using MinecraftEnchantCalculator.Resources.Sqlite;

namespace MinecraftEnchantCalculator.Data.Mapper
{
  public class ConflictsMapper
  {
    public static ConflictsMapper Instance = new();
    private static readonly string _query = string.Format(SqliteConfig.Default.Conflicts,
      nameof(Conflict.EnchantmentCode_1), nameof(Conflict.EnchantmentCode_2));
    private static Dictionary<int, List<int>> _mapper;

    static ConflictsMapper()
    {
      _mapper = new Dictionary<int, List<int>>();

      using SQLiteCommand command = new SQLiteCommand(_query, DBConnection.Connection);
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