using System.Collections.Generic;

namespace TRIdle.Game.Skill
{
  public class Skill_Common : SkillBase<Skill_Common>
  {
    public override string Name => Text.Common.Skill_Common_Name;

    public override IEnumerable<ActionBase> Actions => new ActionBase[] {
      // todo add actions
    };
  }
}