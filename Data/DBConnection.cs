using System.Data.SQLite;
using System.IO;
using MinecraftEnchantCalculator.Resources.Sqlite;

namespace MinecraftEnchantCalculator.Data
{
  public class DBConnection
  {
    private static readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
      SqliteConfig.Default.ResourcesDir, SqliteConfig.Default.SqliteDir, SqliteConfig.Default.Database);
    public static readonly SQLiteConnection Connection =
      new(string.Format(SqliteConfig.Default.ConnectionString, _dbPath));

    // Sqlite 数据库连接实例
    // public static readonly SQLiteConnection Connection = new(string.Format(SqliteConfig.Default.SqliteConnectionString,
    //   AppDomain.CurrentDomain.BaseDirectory));

    static DBConnection()
    {
      Connection.Open();
    }
  }
}