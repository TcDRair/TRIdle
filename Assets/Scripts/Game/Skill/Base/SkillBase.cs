using System.Text.Json.Nodes;
using System.Collections.Generic;

namespace TRIdle.Game.Skill
{
  using Base;

  public abstract class SkillBase : SerializedBase
  {
    public abstract string Description { get; }
    public int Proficiency;

    public abstract IEnumerable<ActionBase> Actions { get; }

    public override sealed void LoadData(JsonNode data) {
      Proficiency = data["proficiency"].GetValue<int>();
      LoadCustomData(data["custom"]);
      var actionNode = data["actions"];
      foreach (var action in Actions) {
        try { action.LoadData(actionNode[action.ID.ToString()]); }
        catch { continue; } // Skip invalid action
      }
    }

    public override sealed JsonNode SaveData() {
      JsonObject node = new(), actionNode = new();
      foreach (var action in Actions)
        actionNode[action.ID.ToString()] = action.SaveData() ?? new JsonObject();
      node["proficiency"] = Proficiency;
      node["custom"] = SaveCustomData() ?? new JsonObject();
      node["actions"] = actionNode;
      return node;
    }
  }
  public abstract class SkillBase<T> : SkillBase, IIdendifiedInstance<T> where T : SkillBase<T>, new()
  {
    public override sealed string ID => IIdendifiedInstance<T>.UID;
    public static T Instance => IIdendifiedInstance<T>.Instance;

  }
}