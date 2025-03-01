using System.Text.Json.Nodes;

namespace TRIdle.Game.Skill
{
  using Base;
  using Logics.Math;
    using TRIdle.Logics.Extensions;

    public abstract class ActionBase : SerializedBase
  {
    public abstract SkillBase BaseSkill { get; }

    public abstract string DescriptionInfo { get; } // Link to Text.Current
    public abstract string DetailedInfo { get; } // Link to Text.Current

    #region Data
    public int Proficiency;
    public float Progress;

    public override sealed void LoadData(JsonNode data) {
      Proficiency = data["proficiency"].GetValue<int>();
      Progress = data["progress"].GetValue<float>();
      LoadCustomData(data["custom"]);
    }

    public override sealed JsonNode SaveData()
      => new JsonObject {
        ["proficiency"] = Proficiency,
        ["progress"] = Progress,
        ["custom"] = SaveCustomData() ?? new JsonObject()
      };
    #endregion

    public void Activate() {
      OnActivated();
      BaseSkill.Proficiency++; // TODO : add proficiency modifier and calculate for each action
      Progress--;
    }

    /// <summary>구현 시 코루틴 관련 메서드를 호출하지 말 것</summary>
    protected abstract void OnActivated();

    public class ValueData
    {
      public RFloat Duration { get; init; }
      public RFloat Speed { get; private set; } = new(1);
    }
    public abstract ValueData Data { get; }
  }

  public abstract class ActionBase<T> : ActionBase, IIdendifiedInstance<T> where T : ActionBase<T>, new()
  {
    public override sealed string ID => IIdendifiedInstance<T>.UID;
    public static T Instance => IIdendifiedInstance<T>.Instance;
  }
}