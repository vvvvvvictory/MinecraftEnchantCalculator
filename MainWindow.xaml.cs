using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MinecraftEnchantCalculator.Data;
using MinecraftEnchantCalculator.Data.Mapper;
using MinecraftEnchantCalculator.Data.ViewModel;
using MinecraftEnchantCalculator.Resources.I18n;

namespace MinecraftEnchantCalculator
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private Dictionary<int, EnchantmentView> _bookEnchantmentViewsCache = new();
    private Dictionary<int, EnchantmentView> _initEnchantmentViewsCache = new();

    public MainWindow()
    {
      InitializeComponent();
      DoTemplateCommandsBinding();
      InitializeDataSource();
    }

    public ObservableCollection<ItemView> ItemViews { get; } = new();                   // 物品选择
    public ObservableCollection<EnchantmentView> InitEnchantmentViews { get; } = new(); // 初始附魔
    public ObservableCollection<EnchantmentView> BookEnchantmentViews { get; } = new(); // 魔咒选择
    public SummaryView Summary { get; } = new();                                        // 结果汇总
    public ObservableCollection<FavoriteView> FavoriteViews { get; } = new();           // 收藏夹
    public ObservableCollection<ResultView> ResultViews { get; } = new();               // 过程展示

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

    private void DoTemplateCommandsBinding()
    {
      CommandBindings.Add(new CommandBinding(TemplateCommands.EnchantLevelIncreasing, LevelIncreasingButton_Click));
      CommandBindings.Add(new CommandBinding(TemplateCommands.EnchantLevelDecreasing, LevelDecreasingButton_Click));
      CommandBindings.Add(new CommandBinding(TemplateCommands.FavoriteViewsDelete, FavoriteViewsDeleteButton_Click));
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

    private void Calculate_Executed(object sender, ExecutedRoutedEventArgs e)
    {
      Summary.Restore();
      ResultViews.Clear();

      // 包含魔咒的物品，包括选中的工具和附魔书
      List<EnchantableItem> items = new List<EnchantableItem>();

      // 初始目标物品
      ItemView selectedItemView = (ItemView)X_ItemViews.SelectedItem;
      EnchantableItem targetItem = new EnchantableItem(ItemType.ITEM, selectedItemView.Item.Code);
      foreach (EnchantmentView initEncView in InitEnchantmentViews) {
        if (initEncView.IsSelected) {
          targetItem.AddEnchantment(new EnchantmentViewPerspective(initEncView));
        }
      }
      items.Add(targetItem);

      // 初始附魔书
      foreach (EnchantmentView bookEncView in BookEnchantmentViews) {
        if (!bookEncView.IsSelected) continue;
        EnchantableItem book = new EnchantableItem(ItemType.BOOK, -1);
        book.AddEnchantment(new EnchantmentViewPerspective(bookEncView));
        items.Add(book);
      }

      // 构建附魔树并统计结果
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

      if (isCancelClick) {
        return;
      }
      if (string.IsNullOrWhiteSpace(input.InputText)) {
        MessageBox.Show(this, "Invalid input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      else {
        string itemCultureKey = ItemViewMapper.Instance[ResultViews[^1].Target.EnchantableItem.ItemId].Item.CultureKey;
        FavoriteView fv = new FavoriteView(input.InputText, itemCultureKey, ResultViews);
        FavoriteViews.Add(fv);

        // 序列化保存文件
        string jsonString = JsonSerializer.Serialize(fv);
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Json",
          $"{fv.DisplayName}.json");
        File.WriteAllText(filePath, jsonString);
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
        MessageBox.Show(this, $"Failed to load Json files, count = {errors}\n{failed!.Message}", "Error",
          MessageBoxButton.OK,
          MessageBoxImage.Error);
      }
    }

    #region Is Boring
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
        MessageBox.Show(this, $"Failed to delete file: {fv.DisplayName}.json, Reason: {e.Message}", "Error",
          MessageBoxButton.OK,
          MessageBoxImage.Error);
      }
    }
    #endregion
  }
}