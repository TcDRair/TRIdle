using System.Collections.Generic;

namespace TRIdle.Game.Skill
{
  public class Skill_Common : SkillBase<Skill_Common>
  {
    public override string ID => "common";
    public override string Name => Text.Current.Skill.Skill_Common_Name;

    public override IEnumerable<ActionBase> Actions => new ActionBase[] {
      // todo add actions
    };
  }
}