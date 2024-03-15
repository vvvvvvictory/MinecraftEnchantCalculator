using MinecraftEnchantCalculator.Data;

namespace MinecraftEnchantCalculator
{
  public class AnvilTree
  {
    private AnvilNode _root;

    private AnvilTree(AnvilNode root)
    {
      _root = root;
    }

    public static AnvilTree BuildTree(List<EnchantableItem> items,
      Action<EnchantableItem, EnchantableItem, EnchantableItem> visualize)
    {
      int n = items.Count;
      if (n < 2) {
        throw new ArgumentException("至少需要两个物品才能附魔");
      }

      // 第一层高度视为0
      int h = 0;
      while (1 << h <= n) {
        if (1 << h == n) {
          break;
        }
        ++h;
      }

      // 构建树
      List<AnvilNode> leafs = new List<AnvilNode>();
      AnvilNode root = BuildTreeDFS(leafs, h + 1, 0, ref n)!;

      // 根据叶子结点权重升序填入降序的可附魔物品，然后计算附魔从而刷新整棵树
      leafs.Sort((x, y) => x.Weight.CompareTo(y.Weight));
      items.Sort((x, y) => y.Value.CompareTo(x.Value));
      for (int i = 0; i < leafs.Count; ++i) {
        leafs[i].EnchantableItem = items[i];
      }

      // 更新整棵树，模拟铁砧附魔过程
      DoEnchant(root, visualize);
      return new AnvilTree(root);
    }

    private static AnvilNode? BuildTreeDFS(List<AnvilNode> leafs, int height, int weight, ref int leafCount)
    {
      // 出口：不需要添加叶子结点或已到达最底层
      if (leafCount == 0 || height == 0) {
        return null;
      }

      AnvilNode ans = new AnvilNode(weight) {
        Left = BuildTreeDFS(leafs, height - 1, weight, ref leafCount),
        Right = BuildTreeDFS(leafs, height - 1, weight + 1, ref leafCount)
      };

      // 叶子结点
      if (ans.Left == null && ans.Right == null) {
        leafs.Add(ans);
        leafCount--;
        return ans;
      }
      // 右子树为空，此时返回左子树，由于尽可能先构建左子树，这里不存在左子树为空，但右子树不为空的情况
      // 同样，由于左子树并不会增加权重，因此即使返回更低层左子树，其根节点的权重等于当前节点的权重，所以
      // 不用当返回左子树时，不用更新节点的权重
      return ans.Right == null ? ans.Left : ans;
    }

    private static void DoEnchant(AnvilNode node, Action<EnchantableItem, EnchantableItem, EnchantableItem> visualize)
    {
      if (node.Left == null && node.Right == null) {
        return;
      }
      DoEnchant(node.Left!, visualize);
      DoEnchant(node.Right!, visualize);
      node.EnchantableItem = node.Left!.EnchantableItem + node.Right!.EnchantableItem;
      visualize(node.Left!.EnchantableItem, node.Right!.EnchantableItem, node.EnchantableItem);
    }

    private class AnvilNode
    {
      public AnvilNode(int weight)
      {
        Weight = weight;
      }

      public EnchantableItem EnchantableItem { get; set; } = null!;
      public int Weight { get; } = 0;

      public AnvilNode? Left { get; init; }
      public AnvilNode? Right { get; init; }
    }
  }
}