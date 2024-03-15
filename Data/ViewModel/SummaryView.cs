using System.ComponentModel;
using System.Runtime.CompilerServices;
using MinecraftEnchantCalculator.Resources.I18n;

namespace MinecraftEnchantCalculator.Data.ViewModel
{
  public class SummaryView : INotifyPropertyChanged
  {
    private int _expCost;
    private bool _isRename;
    private int _maxCost;
    private int _totalStep;

    public SummaryView()
    {
      _totalStep = 0;
      _expCost = 0;
      _isRename = false;
      _maxCost = 0;
      Location.Instance.CultureChanged += () => {
        // 更改是否可行的本地化文字
        OnPropertyChanged(nameof(IsFeasible));
      };
    }

    public int MaxCost {
      get => _maxCost;
      set {
        _maxCost = value;
        OnPropertyChanged(nameof(IsFeasible));
      }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Restore()
    {
      TotalStep = 0;
      ExpCost = 0;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    #region UI Binding Properties
    public int TotalStep {
      get => _totalStep;
      set {
        _totalStep = value;
        OnPropertyChanged(nameof(TotalStep));
      }
    }
    public bool IsFeasible => MaxCost <= 38;
    public int ExpCost {
      get => _expCost;
      set {
        _expCost = value;
        OnPropertyChanged(nameof(ExpCost));
        // 是否可行由花费的总经验决定，因此当总经验变化时，同步更新IsFeasible
        OnPropertyChanged(nameof(IsFeasible));
      }
    }
    public bool IsRename {
      get => _isRename;
      set {
        if (value) {
          ExpCost += 1; // 重命名费用永远为1，且不会增加累计惩罚
        }
        else {
          ExpCost -= 1;
        }
        _isRename = value;
        OnPropertyChanged(nameof(IsRename));
      }
    }
    #endregion
  }
}