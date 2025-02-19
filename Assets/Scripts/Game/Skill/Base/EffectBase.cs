namespace TRIdle.Game.Skill
{
  using Base;

  // ActionBase와 유사한 구조 사용
  // Action과 같이 특정 Skill에 종속되어 있음 (분류를 위해)
  // 추후 Action과의 공통분모는 인터페이스로 분리할 것.
  // 지금 당장은 Action과 Effect의 공통분모가 없어서 인터페이스로 분리할 필요가 없음.
  
  public abstract class EffectBase : IDBase
  {
    public abstract string Description { get; }

    public abstract SkillBase Category { get; }


    public bool IsActive { get; protected set; }

    // Invoked once when the effect is activated
    public abstract void OnActivate();
    // Invoked every frame while the effect is active
    public abstract void OnUpdate();
    // Invoked once when the effect is deactivated
    public abstract void OnDeactivate();
  }

  public abstract class EffectBase<T> : EffectBase, IInst<T> where T : EffectBase<T>, new()
  {
    public override int ID => nameof(T).GetHashCode();
    public static T Instance => IInst<T>.Instance;
  }
}