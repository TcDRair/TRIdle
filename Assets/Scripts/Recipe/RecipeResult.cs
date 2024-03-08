

using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using TRIdle.Game.Item;

using UnityEngine;

namespace TRIdle.Game.Recipe
{
  public abstract class RecipeResult : MonoBehaviour
  {
    // On Item Added/Removed from the list.
    public delegate void GetResult(ExpectedResult result);
    public event GetResult OnItemChanged;

    protected RecipeSerialized info;
    private ExpectedResult result;
    protected TItem[][] items;
    protected TItem[][] tools;
    /// <summary>
    /// Expect the result of the recipe with the given items and tools.
    /// Some items or tools may be null.
    /// </summary>
    /// <returns>Returns the expected result of the recipe.</returns>

    protected abstract ExpectedResult ExpectedResult();

    #region Pre-Creation
    public void SetRecipe(RecipeSerialized info) {
      this.info = info;
      items = new TItem[info.requirements.Length][];
      tools = new TItem[info.tools.Length][];
      for (int i = 0; i < info.requirements.Length; i++) {
        items[i] = new TItem[info.requirements[i].amount];
      }
      for (int i = 0; i < info.tools.Length; i++) {
        tools[i] = new TItem[info.tools[i].amount];
      }
      result = ExpectedResult();
      OnItemChanged?.Invoke(result);
    }
    public void SetItem(TItem item, int idx1, int idx2) {
      Validate(item, info.requirements[idx1]);

      items[idx1][idx2] = item;
      result = ExpectedResult();
      OnItemChanged?.Invoke(result);
    }
    public void SetTool(TItem item, int idx1, int idx2) {
      Validate(item, info.tools[idx1]);

      tools[idx1][idx2] = item;
      result = ExpectedResult();
      OnItemChanged?.Invoke(result);
    }
    private void Validate(TItem item, RequiredItem req) {
      // Level check
      if (req.levelRange.use &&
         (item.info.level < req.levelRange.min ||
          item.info.level > req.levelRange.max))
        return; // TODO: Show error message
      // Tag check
      if (req.tags.Count > 0) {
        bool hasTag = false;
        foreach (var reqTag in req.tags) {
          if (item.tags.Any(t => t.data.Equals(reqTag.data))) {
            hasTag = true;
            break;
          }
        }
        if (!hasTag) return; // TODO: Show error message
      }
      // TODO: Attribute check
    }
    #endregion

    #region Post-Creation
    public abstract TItem RunRecipe();
    #endregion
  }

  public struct ExpectedResult
  {
    public string name;
    public int amount;
    public float successRate;
    public Dictionary<int, float> levelRange;
    public ExpectedTag[] tags;
    // Attributes are not considered for now
  }
  public struct ExpectedTag
  {
    public TagData tag;
    public float[] levelRange;
  }
}