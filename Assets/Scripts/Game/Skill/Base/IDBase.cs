using System.Text.Json.Nodes;

namespace TRIdle.Game.Skill.Base
{
  public abstract class SerializedBase
  {
    protected SerializedBase() { } // Prevent external instantiation of this class

    public abstract string ID { get; } // 구현 클래스의 고유 식별자로 사용됨
    public abstract string Name { get; } // Text에 연결되어 표기 텍스트를 불러옴

    protected Localization.Text_Skills Text => TextLocale.Current.Skills;

    public abstract void LoadData(JsonNode data);
    protected abstract void LoadCustomData(JsonNode data);
    public abstract JsonNode SaveData();
    protected abstract JsonNode SaveCustomData();

    public override bool Equals(object obj)
      => obj is SerializedBase element && ID.Equals(element.ID);
    public override int GetHashCode() => ID.GetHashCode();
  }

  public interface IInst<T> where T : IInst<T>, new()
  {
    private static T m_instance;
    public static T Instance => m_instance ??= new();
  }
}