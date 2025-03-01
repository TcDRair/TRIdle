using System.Text.Json.Nodes;

using UnityEngine;

namespace TRIdle.Game.Skill
{
  using Logics.Math;
  using Logics.Extensions;

  public class Action_Wildcrafting_Search : ActionBase<Action_Wildcrafting_Search>
  {
    public override SkillBase BaseSkill => Skills.Wildcrafting;

    public override string Name => Text.Wildcrafting.Action_Wildcrafting_Search_Name;
    public override string DescriptionInfo => Text.Wildcrafting.Action_Wildcrafting_Search_DescriptionInfo;
    public override string DetailedInfo => string.Format(Text.Wildcrafting.Action_Wildcrafting_Search_DetailedInfo, Amount);

    private float Amount => Mathf.Clamp(Mathf.Pow(Proficiency, 0.95f) / 10, 0, 66);
    private SFloat DurSpdProf(SFloat value) {
      value.multiplier -= Amount / 100;
      return value;
    }
    public Action_Wildcrafting_Search() {
      m_data = new ValueData() { Duration = new(3, 1) };
      m_data.Duration.Modifiers += DurSpdProf;
    }
    private readonly ValueData m_data;
    public override ValueData Data => m_data;

    protected override void OnActivated() {
      Proficiency += 1;
      this.Log($"Action has been activated.");
    }

    protected override void LoadCustomData(JsonNode data) { }
    protected override JsonNode SaveCustomData() => null;
  }
}