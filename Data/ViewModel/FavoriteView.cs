using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace MinecraftEnchantCalculator.Data.ViewModel
{
  public class FavoriteView
  {
    public FavoriteView(string displayName, string cultureKey, ObservableCollection<ResultView> resultViews)
    {
      DisplayName = displayName;
      ImageSource = $"pack://application:,,,/MinecraftEnchantCalculator;component/Resources/images/{cultureKey}.png";
      ResultViewProspectives = new List<ResultViewProspective>();
      foreach (ResultView rv in resultViews) {
        ResultViewProspectives.Add(new ResultViewProspective(rv));
      }
    }

    [JsonConstructor]
    public FavoriteView(string displayName, string imageSource, List<ResultViewProspective> resultViewProspectives)
    {
      DisplayName = displayName;
      ImageSource = imageSource;
      ResultViewProspectives = resultViewProspectives;
    }

    public string DisplayName { get; set; }
    public string ImageSource { get; set; }
    public List<ResultViewProspective> ResultViewProspectives { get; set; }
  }
}