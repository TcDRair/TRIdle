using System.Collections.Generic;

namespace TRIdle.Game.Skill
{
  using Base;

  public abstract class SkillBase : IDBase
  {
    public abstract IEnumerable<ActionBase> Actions { get; }
  }
  public abstract class SkillBase<T> : SkillBase, IInst<T> where T : SkillBase<T>, new()
  {
    public override int ID => typeof(T).GetHashCode();
    public static T Instance => IInst<T>.Instance;
  }
}