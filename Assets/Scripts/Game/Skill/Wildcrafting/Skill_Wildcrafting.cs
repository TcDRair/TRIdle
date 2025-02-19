using System.Collections.Generic;

namespace TRIdle.Game.Skill
{
  public class Skill_Wildcrafting : SkillBase<Skill_Wildcrafting>
  {
    public override string Name => TextLocale.Current.Skill.Skill_Wildcrafting_Name;

    public override IEnumerable<ActionBase> Actions => new ActionBase[] {
      Action_Wildcrafting_Search.Instance
    };
  }
}