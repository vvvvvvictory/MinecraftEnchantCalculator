using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MinecraftEnchantCalculator.Components;
using MinecraftEnchantCalculator.Data;
using MinecraftEnchantCalculator.Data.Mapper;
using MinecraftEnchantCalculator.Data.ViewModel;
using MinecraftEnchantCalculator.Pages;
using MinecraftEnchantCalculator.Resources.I18n;
using MinecraftEnchantCalculator.Resources.Settings;
using MinecraftEnchantCalculator.Source;

namespace MinecraftEnchantCalculator
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private static readonly ImageSource _error =
      new BitmapImage(
        new Uri("pack://application:,,,/MinecraftEnchantCalculator;component/Resources/images/PopError.png"));
    private static readonly ImageSource _info =
      new BitmapImage(
        new Uri("pack://application:,,,/MinecraftEnchantCalculator;component/Resources/images/PopInfo.png"));
    private static readonly ImageSource _success =
      new BitmapImage(
        new Uri("pack://application:,,,/MinecraftEnchantCalculator;component/Resources/images/PopSuccess.png"));
    private static readonly ImageSource _warning =
      new BitmapImage(
        new Uri("pack://application:,,,/MinecraftEnchantCalculator;component/Resources/images/PopWarning.png"));

    private Dictionary<int, EnchantmentView> _bookEnchantmentViewsCache = new(); // 物品附魔缓存
    private Dictionary<int, EnchantmentView> _initEnchantmentViewsCache = new(); // 附魔书视图缓存
    private DispatcherTimer _msgDurationTimer = new();                           // MessagePopup 显示时长计时器

    public MainWindow()
    {
      InitializeComponent();
      DoTemplateCommandsBinding(); // 初始化模板中的命令绑定
      InitializeDataSource();      // 初始化组件数据源
      InitMsgDurationTimer();      // 初始化消息框计时器
      InitLocation();              // 初始化区域信息
    }

    public ObservableCollection<ItemView> ItemViews { get; } = new();                   // 物品选择
    public ObservableCollection<EnchantmentView> InitEnchantmentViews { get; } = new(); // 初始附魔
    public ObservableCollection<EnchantmentView> BookEnchantmentViews { get; } = new(); // 魔咒选择
    public SummaryView Summary { get; } = new();                                        // 结果汇总
    public ObservableCollection<FavoriteView> FavoriteViews { get; } = new();           // 收藏夹
    public ObservableCollection<ResultView> ResultViews { get; } = new();               // 过程展示

    private void InitMsgDurationTimer()
    {
      _msgDurationTimer.Interval = TimeSpan.FromSeconds(5); // 默认显示5秒
      _msgDurationTimer.Tick += (_, _) => {
        // 到达指定时间后停止计时并隐藏消息框
        _msgDurationTimer.Stop();
        X_MessagePopup.Opacity = 0;
      };
    }

    private void InitLocation()
    {
      // 设置界面语言，默认中文启动
      string locationAbbrName = ConfigurationManager.AppSettings["LocationInfo"] ?? "zh-CN";
      Location.Instance.ChangeLocation(new CultureInfo(locationAbbrName));

      // 监听区域更改事件，保存用户选择
      Location.Instance.RegionInfoChanged += regionAbbrName => {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings["LocationInfo"].Value = regionAbbrName;
        config.Save(ConfigurationSaveMode.Modified);
      };
    }

    private void InitializeDataSource()
    {
      List<ItemView> itemViews = ItemViewMapper.Instance.ViewModels();
      foreach (ItemView itemView in itemViews) {
        ItemViews.Add(itemView);
      }

      // 初始化视图缓存
      foreach (EnchantmentView ev in PrototypeEnchantmentViewMapper.Instance.ViewModels()) {
        ev.PropertyChanged += (sender, args) => {
          if (args.PropertyName != nameof(EnchantmentView.IsSelected)) {
            return;
          }

          EnchantmentView thisView = (EnchantmentView)sender!;
          int code = thisView.PrototypeEnchantmentView.Enchantment.Code;
          if (_bookEnchantmentViewsCache[code].IsSelected) {
            // 如果同名的附魔书被选中，说明互斥的魔咒应该也要被禁用，此时不作任何操作
            return;
          }

          ShiftMutex(code, !thisView.IsSelected);
        };
        _initEnchantmentViewsCache.Add(ev.PrototypeEnchantmentView.Enchantment.Code, ev);
      }
      foreach (EnchantmentView ev in PrototypeEnchantmentViewMapper.Instance.ViewModels()) {
        ev.PropertyChanged += (sender, args) => {
          if (args.PropertyName != nameof(EnchantmentView.IsSelected)) {
            return;
          }

          EnchantmentView thisView = (EnchantmentView)sender!;
          int code = thisView.PrototypeEnchantmentView.Enchantment.Code;
          if (_initEnchantmentViewsCache[code].IsSelected) {
            return; // 原因同上
          }

          ShiftMutex(code, !thisView.IsSelected);
        };
        _bookEnchantmentViewsCache.Add(ev.PrototypeEnchantmentView.Enchantment.Code, ev);
      }
    }

    private void PushMessage(MessageType msgType, string msg)
    {
      // 停止计时器
      _msgDurationTimer.Stop();

      // 更新MessagePopup属性
      X_MessagePopup.MessageType = msgType;
      X_MessagePopup.Message = msg;
      X_MessagePopup.Opacity = 1; // 重新显示气泡框
      X_MessagePopup.Icon = msgType switch {
        MessageType.ERROR => _error,
        MessageType.WARNING => _warning,
        MessageType.INFO => _info,
        MessageType.SUCCESS => _success,
        _ => throw new ArgumentOutOfRangeException(nameof(msgType), msgType, null)
      };

      _msgDurationTimer.Start(); // 重新开始计时
    }

    private void DoTemplateCommandsBinding()
    {
      CommandBindings.Add(new CommandBinding(TemplateCommands.EnchantLevelIncreasing, LevelIncreasingButton_Click));
      CommandBindings.Add(new CommandBinding(TemplateCommands.EnchantLevelDecreasing, LevelDecreasingButton_Click));
      CommandBindings.Add(new CommandBinding(TemplateCommands.FavoriteViewsDelete, FavoriteViewsDeleteButton_Click));
      CommandBindings.Add(new CommandBinding(AppCommands.StopBoringCommand, StopRotationAnimation));
    }

    private void ShiftMutex(int encCode, bool state)
    {
      // 如果当前魔咒存在互斥的魔咒，那么在选中时禁用所有互斥的项，
      // 非选中时启用所有互斥项
      List<int>? mutex = ConflictsMapper.Instance[encCode];
      if (mutex == null) return;
      foreach (int cftCode in mutex) {
        // 直接从原型上禁用
        PrototypeEnchantmentViewMapper.Instance[cftCode].IsEnabled = state;
      }
    }

    private void X_ItemViews_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (X_ItemViews.SelectedItem == null) {
        return;
      }

      // 清空列表
      InitEnchantmentViews.Clear();
      BookEnchantmentViews.Clear();

      ItemView itemView = (ItemView)X_ItemViews.SelectedItem;
      List<int> targetEncCodes = ApplicableMapper.Instance[itemView.Item.Code]!;
      // 重新添加
      foreach (int code in targetEncCodes) {
        EnchantmentView initEncView = _initEnchantmentViewsCache[code];
        initEncView.IsSelected = false;
        InitEnchantmentViews.Add(initEncView);

        EnchantmentView bookEncView = _bookEnchantmentViewsCache[code];
        bookEncView.IsSelected = false;
        BookEnchantmentViews.Add(bookEncView);
      }
    }

    private void X_FavoriteViews_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (X_FavoriteViews.SelectedItem == null) {
        return;
      }

      ResultViews.Clear();
      Summary.Restore();

      try {
        int cost = 0;
        int maxCost = 0;
        FavoriteView fv = (FavoriteView)X_FavoriteViews.SelectedItem;
        foreach (ResultViewProspective rvp in fv.ResultViewProspectives) {
          ResultViews.Add(rvp.ConvertBack());
          cost += rvp.TargetCost;
          maxCost = Math.Max(maxCost, rvp.TargetCost);
        }
        Summary.TotalStep = fv.ResultViewProspectives.Count;
        Summary.ExpCost = cost;
        Summary.MaxCost = maxCost;
      }
      catch (Exception exception) {
        PushMessage(MessageType.ERROR, Location.Instance[LanguageKey.Default.LoadFavoriteFailed] ?? "ERROR");
        MessageBox.Show(this, $"无法加载收藏项：错误消息{exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      // 加载成功
      PushMessage(MessageType.SUCCESS, Location.Instance[LanguageKey.Default.LoadFavoriteSuccess] ?? "SUCCESS");
    }

    private void Calculate_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Summary.Restore();
      ResultViews.Clear();

      // 包含魔咒的物品，包括选中的工具和附魔书
      List<EnchantableItem> items = new List<EnchantableItem>();

      // 初始目标物品
      // 这里selectedItemView一定不可能为null，因为我们在该命令的可执行逻辑中
      // 确保了至少选择了一项可附魔物品
      ItemView selectedItemView = (ItemView)X_ItemViews.SelectedItem;
      EnchantableItem targetItem = new EnchantableItem(ItemType.ITEM, selectedItemView.Item.Code);
      foreach (EnchantmentView initEncView in InitEnchantmentViews) {
        if (initEncView.IsSelected) {
          targetItem.AddEnchantment(new EnchantmentViewPerspective(initEncView));
        }
      }
      items.Add(targetItem);

      // 初始附魔书
      // 同样，在该命令的执行逻辑中，我们已经确保至少选择了一本附魔书
      foreach (EnchantmentView bookEncView in BookEnchantmentViews) {
        if (!bookEncView.IsSelected) continue;
        EnchantableItem book = new EnchantableItem(ItemType.BOOK, -1);
        book.AddEnchantment(new EnchantmentViewPerspective(bookEncView));
        items.Add(book);
      }

      // 构建附魔树并统计结果
      try {
        int step = 0;
        int totalCost = 0;
        int maxCost = 0;
        AnvilTree.BuildTree(items,
          (l, r, t) => {
            ResultViews.Add(new ResultView(
              new EnchantableItemView(l),
              new EnchantableItemView(r),
              new EnchantableItemView(t)));
            step++;
            totalCost += t.Cost;
            maxCost = Math.Max(maxCost, t.Cost);
          });
        Summary.TotalStep = step;
        Summary.MaxCost = maxCost;
        Summary.ExpCost = totalCost;
      }
      catch (Exception ex) {
        // 如果构建过程中不小心出现意外的错误，打印错误信息，用于排查，然后退出程序
        MessageBox.Show(this, $"附魔过程中出现了意外异常，请尝试重新打开，异常信息：{ex.Message}", "Error", MessageBoxButton.OK);
        Close();
      }
      // 附魔成功
      PushMessage(MessageType.SUCCESS, Location.Instance[LanguageKey.Default.CalculatePressed] ?? "SUCCESS");
    }

    private void Calculate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      // 至少选择一个物品和魔咒
      e.CanExecute = X_ItemViews.SelectedItem != null && X_BookEnchantmentViews.SelectedItems.Count > 0;
    }

    private void Favorite_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      FavoriteNameInput input = new FavoriteNameInput { Owner = this };
      bool isCancelClick = false;
      input.CancelButtonClick += () => { isCancelClick = true; };
      input.ShowDialog();

      // 取消收藏
      if (isCancelClick) {
        PushMessage(MessageType.INFO, Location.Instance[LanguageKey.Default.FavoriteNameDiscard] ?? "INFO");
        return;
      }
      if (string.IsNullOrWhiteSpace(input.InputText)) {
        // 点击关闭
        PushMessage(MessageType.WARNING, Location.Instance[LanguageKey.Default.FavoriteNameInputClosed] ?? "ERROR");
      }
      else {
        string trim = input.InputText.Trim();
        bool isPresent = FavoriteViews.Any(fv => fv.DisplayName == trim);
        if (isPresent) {
          PushMessage(MessageType.WARNING, Location.Instance[LanguageKey.Default.FavoriteHasPresent] ?? "WARNING");
          return;
        }
        string itemCultureKey = ItemViewMapper.Instance[ResultViews[^1].Target.EnchantableItem.ItemId].Item.CultureKey;
        FavoriteView fv = new FavoriteView(input.InputText, itemCultureKey, ResultViews);
        FavoriteViews.Add(fv);

        // 序列化保存文件
        try {
          string jsonString = JsonSerializer.Serialize(fv);
          string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Json",
            $"{fv.DisplayName}.json");
          File.WriteAllText(filePath, jsonString);
        }
        catch (Exception exception) {
          FavoriteViews.Remove(fv);
          PushMessage(MessageType.ERROR, Location.Instance[LanguageKey.Default.AddFavoriteFailed] ?? "ERROR");
          MessageBox.Show(this, $"无法写入文件：错误消息{exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // 收藏成功
        PushMessage(MessageType.SUCCESS, Location.Instance[LanguageKey.Default.AddFavoriteSuccess] ?? "SUCCESS");
      }
    }

    private void Favorite_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
    {
      // 至少有一个附魔的过程才能收藏
      e.CanExecute = ResultViews.Count > 0;
    }

    private void X_FavoriteViews_OnLoaded(object sender, RoutedEventArgs e)
    {
      string[] jsonFiles = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Json"),
        "*.json");
      int errors = 0;
      Exception failed = null!;
      foreach (string jsonFile in jsonFiles) {
        try {
          FavoriteView fv = JsonSerializer.Deserialize<FavoriteView>(File.ReadAllText(jsonFile))!;
          FavoriteViews.Add(fv);
        }
        catch (Exception ex) {
          errors++;
          failed = ex;
        }
      }
      if (errors != 0) {
        MessageBox.Show(this, $"Failed to load Json files, count = {errors}\n{failed.Message}", "Error",
          MessageBoxButton.OK,
          MessageBoxImage.Error);
      }
    }

    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
      new ViewTest().Show();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
      // 显示欢迎信息
      string welcomeKey = LanguageKey.Default.Welcome;
      PushMessage(MessageType.INFO, Location.Instance[welcomeKey] ?? welcomeKey);
    }

    #region Is Boring
    private HashSet<GroupBox> _rotateGroupBoxes = new();

    private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
      switch (e.Key) {
        case Key.F1:
          StartRotationAnimation(X_ItemGroupBox);
          break;
        case Key.F2:
          StartRotationAnimation(X_InitEnchantmentGroupBox);
          break;
        case Key.F3:
          StartRotationAnimation(X_BookEnchantmentGroupBox);
          break;
        case Key.F4:
          StartRotationAnimation(X_SummaryGroupBox);
          break;
        case Key.F5:
          StartRotationAnimation(X_FavoriteGroupBox);
          break;
      }
    }

    private void StartRotationAnimation(GroupBox targetGroupBox)
    {
      if (!_rotateGroupBoxes.Add(targetGroupBox)) {
        return;
      }
      DoubleAnimation animation = new DoubleAnimation {
        From = 0,
        To = 360,
        Duration = TimeSpan.FromSeconds(2),
        RepeatBehavior = RepeatBehavior.Forever
      };
      RotateTransform rotateTransform = new RotateTransform();
      targetGroupBox.RenderTransformOrigin = new Point(0.5, 0.5);
      targetGroupBox.RenderTransform = rotateTransform;
      rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
    }

    private void StopRotationAnimation(object sender, ExecutedRoutedEventArgs args)
    {
      if (_rotateGroupBoxes.Count < 1) {
        return;
      }
      foreach (GroupBox gb in _rotateGroupBoxes) {
        Transform gbRenderTransform = gb.RenderTransform;
        gbRenderTransform.BeginAnimation(RotateTransform.AngleProperty, null);
        gb.RenderTransform = null;
      }
      _rotateGroupBoxes.Clear();
    }
    #endregion

    #region Menu Item Clicked
    private void Language_Changed(object sender, RoutedEventArgs e)
    {
      object tag = ((FrameworkElement)sender).Tag;
      Location.Instance.ChangeLocation(new CultureInfo((string)tag));
    }

    private void About_OnClick(object sender, RoutedEventArgs e)
    {
      new About { Owner = this }.ShowDialog();
    }

    private void Donate_OnClick(object sender, RoutedEventArgs e)
    {
      new Donate { Owner = this }.ShowDialog();
    }
    #endregion

    #region Commands Target Execute
    private void LevelIncreasingButton_Click(object sender, ExecutedRoutedEventArgs args)
    {
      EnchantmentView encView = (EnchantmentView)args.Parameter;
      encView.IsSelected = true;
      encView.Level++;
    }

    private void LevelDecreasingButton_Click(object sender, ExecutedRoutedEventArgs args)
    {
      EnchantmentView encView = (EnchantmentView)args.Parameter;
      encView.IsSelected = true;
      encView.Level--;
    }

    private void FavoriteViewsDeleteButton_Click(object sender, ExecutedRoutedEventArgs args)
    {
      FavoriteView fv = (FavoriteView)args.Parameter;
      FavoriteViews.Remove(fv);

      // 从磁盘删除
      string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Json",
        $"{fv.DisplayName}.json");

      try {
        File.Delete(jsonFilePath);
      }
      catch (Exception e) {
        PushMessage(MessageType.ERROR, Location.Instance[LanguageKey.Default.DeleteFavoriteFailed] ?? "ERROR");
        MessageBox.Show(this, $"无法删除文件: {fv.DisplayName}.json, Reason: {e.Message}", "Error",
          MessageBoxButton.OK,
          MessageBoxImage.Error);
      }

      // 删除成功
      PushMessage(MessageType.SUCCESS, Location.Instance[LanguageKey.Default.DeleteFavoriteSuccess] ?? "SUCCESS");
    }
    #endregion
  }
}