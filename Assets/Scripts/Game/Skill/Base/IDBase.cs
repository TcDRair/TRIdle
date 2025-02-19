
namespace TRIdle.Game.Skill.Base
{
  public abstract class IDBase
  {
    protected IDBase() { } // Prevent external instantiation of this class

    public abstract int ID { get; } // 구현 클래스의 고유 식별자로 사용됨
    public abstract string Name { get; } // Text에 연결되어 표기 텍스트를 불러옴

    protected Localization.Text_Skill Text => TextLocale.Current.Skill;

    public override bool Equals(object obj)
      => obj is IDBase element && ID.Equals(element.ID);
    public override int GetHashCode() => ID.GetHashCode();
  }
  
  public interface IInst<T> where T : IInst<T>, new()
  {
    private static T m_instance;
    public static T Instance => m_instance ??= new();
  }
}