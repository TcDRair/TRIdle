using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace TRIdle.Game.Skill
{
  public class Skill_Common : SkillBase<Skill_Common>
  {
    public override string Name => Text.Common.Skill_Common_Name;

    public override IEnumerable<ActionBase> Actions => new ActionBase[] {
      // todo add actions
    };


    protected override void LoadCustomData(JsonNode data) {
      // todo load data (exclude actions)
    }

    protected override JsonNode SaveCustomData() => null;
  }
}