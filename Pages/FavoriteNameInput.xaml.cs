using System.ComponentModel;
using System.Windows;

namespace MinecraftEnchantCalculator.Pages
{
  /// <summary>
  ///   FavoriteNameInput.xaml 的交互逻辑
  /// </summary>
  public partial class FavoriteNameInput : Window
  {
    public Action? CancelButtonClick;

    public FavoriteNameInput()
    {
      InitializeComponent();
    }

    public string InputText { get; private set; } = string.Empty;

    private void Ok_OnClick(object sender, RoutedEventArgs e)
    {
      // 关闭窗口，在FavoriteNameInput_OnClosing函数中设置InputText的值
      Close();
    }

    private void Cancel_OnClick(object sender, RoutedEventArgs e)
    {
      // 关闭窗口，在FavoriteNameInput_OnClosing函数中设置InputText的值
      CancelButtonClick?.Invoke();
      Close();
    }

    private void FavoriteNameInput_OnClosing(object? sender, CancelEventArgs e)
    {
      InputText = X_Input.Text;
    }

    private void FavoriteNameInput_OnLoaded(object sender, RoutedEventArgs e)
    {
      // 加载窗口后让输入框获得焦点
      X_Input.Focus();
    }
  }
}