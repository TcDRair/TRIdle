namespace TRIdle.Game.Skill
{
  using Base;
  using Logics.Math;

  public abstract class ActionBase : IDBase
  {
    public abstract string DescriptionInfo { get; } // Link to Text.Current
    public abstract string DetailedInfo { get; } // Link to Text.Current

    #region Serialized Data
    public int Proficiency;
    public float Progress;
    #endregion

    public void Activate() {
      OnActivated();
      Progress--;
    }

    /// <summary>구현 시 코루틴 관련 메서드를 호출하지 말 것</summary>
    protected abstract void OnActivated();

    public class ValueData
    {
      public RFloat Duration { get; init; }
      public RFloat Speed { get; private set; } = new(1);
    }
    public abstract ValueData Data { get; }
  }

  public abstract class ActionBase<T> : ActionBase, IInst<T> where T : ActionBase<T>, new()
  {
    public override int ID => nameof(T).GetHashCode();
    public static T Instance => IInst<T>.Instance;
  }
}