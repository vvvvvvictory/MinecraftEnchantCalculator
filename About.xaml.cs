using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace MinecraftEnchantCalculator
{
  /// <summary>
  ///   About.xaml 的交互逻辑
  /// </summary>
  public partial class About : Window
  {
    public About()
    {
      InitializeComponent();
    }

    private void OpenSource_Hyperlink(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });
      e.Handled = true;
    }
  }
}