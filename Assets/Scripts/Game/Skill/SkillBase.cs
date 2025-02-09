namespace TRIdle.Game.Skill
{
  public abstract class SkillBase
  {
    public abstract string ID { get; } // ID는 각 액션의 고유 식별자로 사용됨
    public abstract string Name { get; } // Link to Text.Current

    public int Exp;
  }
  public abstract class SkillBase<T> : SkillBase where T : SkillBase<T>, new()
  {
    private static T m_instance;
    public static T Instance => m_instance ??= new();

    protected SkillBase() { } // Prevent external instantiation of this class
  }

  public sealed class Wildcrafting : SkillBase<Wildcrafting>
  {
    public override string ID => "wildcrafting";
    public override string Name => Text.Current.Skill.Skill_Wildcrafting_Name;
  }
}