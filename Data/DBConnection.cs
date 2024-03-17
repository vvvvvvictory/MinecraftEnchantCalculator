using System.Data.SQLite;
using System.IO;
using System.Windows;
using MinecraftEnchantCalculator.Resources.Sqlite;

namespace MinecraftEnchantCalculator.Data
{
  public class DBConnection
  {
    public static readonly SQLiteConnection Connection = null!;
    private static readonly string _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
      SqliteConfig.Default.ResourcesDir, SqliteConfig.Default.SqliteDir, SqliteConfig.Default.Database);

    // Sqlite 数据库连接实例
    // public static readonly SQLiteConnection Connection = new(string.Format(SqliteConfig.Default.SqliteConnectionString,
    //   AppDomain.CurrentDomain.BaseDirectory));

    static DBConnection()
    {
      try {
        Connection = new SQLiteConnection(string.Format(SqliteConfig.Default.ConnectionString, _dbPath));
        Connection.Open();
      }
      catch (Exception) {
        MessageBox.Show("程序缺少启动的必须资源", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Application.Current.Shutdown(-1);
      }
    }
  }
}