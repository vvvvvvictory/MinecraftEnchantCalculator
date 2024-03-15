using System.Data;
using System.Windows;
using MinecraftEnchantCalculator.Data;

namespace MinecraftEnchantCalculator
{
  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      // close sqlite connection before application is closed
      Exit += (_, _) => {
        if (DBConnection.Connection.State == ConnectionState.Open) {
          DBConnection.Connection.Close();
        }
      };
    }
  }
}