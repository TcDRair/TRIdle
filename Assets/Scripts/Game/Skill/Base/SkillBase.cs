using System.Text.Json.Nodes;
using System.Collections.Generic;

namespace TRIdle.Game.Skill
{
  using Base;

  public abstract class SkillBase : SerializedBase
  {
    public abstract IEnumerable<ActionBase> Actions { get; }

    public override sealed void LoadData(JsonNode data) {
      LoadCustomData(data["custom"]);
      var actionNode = data["actions"];
      foreach (var action in Actions)
        action.LoadData(actionNode[action.ID.ToString()]);
    }

    public override sealed JsonNode SaveData() {
      JsonObject node = new(), actionNode = new();
      foreach (var action in Actions)
        actionNode[action.ID.ToString()] = action.SaveData() ?? new JsonObject();
      node["custom"] = SaveCustomData() ?? new JsonObject();
      node["actions"] = actionNode;
      return node;
    }
  }
  public abstract class SkillBase<T> : SkillBase, IInst<T> where T : SkillBase<T>, new()
  {
    public override string ID => typeof(T).Name;
    public static T Instance => IInst<T>.Instance;

  }
}