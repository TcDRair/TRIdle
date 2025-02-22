using System.Text.Json.Nodes;
using System.Collections.Generic;

namespace TRIdle.Game.Skill
{
  public class Skill_Wildcrafting : SkillBase<Skill_Wildcrafting>
  {
    public override string Name => TextLocale.Current.Skills.Skill_Wildcrafting_Name;

    public override IEnumerable<ActionBase> Actions => new ActionBase[] {
      Action_Wildcrafting_Search.Instance
    };

    protected override void LoadCustomData(JsonNode data) {
    }

    protected override JsonNode SaveCustomData() => null;
  }
}