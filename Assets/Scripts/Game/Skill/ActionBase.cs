namespace TRIdle.Game.Skill
{
  public abstract class ActionBase
  {
    public string ID { get; } // ID는 각 액션의 고유 식별자로 사용됨
    public abstract string Name { get; } // Link to Text.Current
    public abstract string DescriptionInfo { get; } // Link to Text.Current
    public abstract string DetailedInfo { get; } // Link to Text.Current

    protected Texts.Skill Texts => Text.Current.Skill;

    #region Serialized Data
    public int Proficiency;
    public float Progress;
    #endregion

    public abstract float Delay { get; }

    public void Activate() {
      OnActivated();
      Progress -= 1;
    }

    /// <summary>구현 시 코루틴 관련 메서드를 호출하지 말 것</summary>
    protected abstract void OnActivated();

    // 추상 클래스에서 정의되는 것
    //  - ID : 아래 항목들과 연결시키기 위한 것
    //  - Name : 언어 설정에 따라 로드된 텍스트를 Text.Current를 통해 레퍼런스할 것
    //  - Exp : 모든 액션은 개별 경험치(숙련도)를 가짐
    //  - Press : 액션 버튼을 눌렀을 때 호출되는 메소드. 입력 시의 즉각적인 로직
    //  - Execute : 액션 실행 시 호출되는 메소드. 액션을 한 번 완료했을 때의 로직
    // 따로 빼서 정의해야 하는 것
    //  - 각종 string => Text : 각 언어별로 로드되어 있음.
    //    Name과 같이 상속받은 액션에서 정의한 후 Text.Current에서 레퍼런스할 것
    //  - UI => ActionPanel : 최소한의 공통 요소를 제외한 UI는 각자 정의해야 함
    //    각 스킬마다 고유한 UI가 많을 수 있어 ActionPanel 등의 클래스를 따로 만드는 것을 권장함
    //    초기 단계에서는 해당 Panel에 텍스트 위주로 구성해보자.
    //  - Data : 저장되어야 하는 데이터는 Text와 같이 일괄적으로 저장되어 있음. (레퍼런스 필요)
    // 상속받은 액션에서 알아서 정의해야 하는 것
    //  - Delay/Cooldown 등 : 액션의 특성에 따라 정의해야 함
    //    기본적인 진행 요소는 Player에서 정의되어 있음 (ex: DoActionDelay(), DoActionCooldown())
    //  - UI : 액션에 따라 UI가 다르므로 상속받은 액션에서 정의해야 함
    //    여기에 쓰긴 했지만 ActionPanel 등의 클래스를 따로 만드는 것을 권장함
    // ...좋았어 벌목(관측 + 탐사 + 채집 + 액션 + 복귀) 스킬에서 프로토타입을 만들어 보자.
  }

  public abstract class ActionBase<T> : ActionBase where T : ActionBase<T>, new()
  {
    private static T m_instance;
    public static T Instance => m_instance ??= new();

    protected ActionBase() { } // Prevent external instantiation of this class
  }
}