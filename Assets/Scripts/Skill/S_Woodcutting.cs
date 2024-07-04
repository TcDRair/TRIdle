using System.Linq;
using System.Text.Json.Serialization;

namespace TRIdle.Game.Skill {
  using Knowledge;

  //! Sample Skill
  public class SB_WoodCutting : SkillBase {
    public override string Name => "벌목";
    public override string Description =>
      $"나무를 베어서 재료를 얻는 스킬\n" +
      $"재생 한도 : {Stats.WoodRegenMax}\n" +
      $"재생 속도 : {Stats.WoodRegenRate}/s\n" +
      $"목재 등급 가중치 : [ {string.Join(" / ", Stats.WoodRankWeight)} ]\n" +
      (Stats.SpiritEnabled is false ? "???" :
      (
        $"정령 사용 가능 : {Stats.SpiritEnabled}\n" +
        $"정령 호의 : {Stats.SpiritFavor}"
      ));
    protected override string IconPath => "Sprite/WoodCutting";
    public override void Initialize() {
      Level = 0;
      MaxLevel = 10;

      Actions = new ActionBase[] {
        new SA_WoodCutting(),
        new SA_StickGathering()
      };
    }

    public record Stat : StatBase {
      public REInt WoodRegenMax { get; set; } = new(100);
      public REFloat WoodRegenRate { get; set; } = new(0.1f);

      public int[] WoodRankWeight { get; set; } = { 90, 9, 1, 0, 0 };

      // TODO : Spirit of the Forest
      public bool SpiritEnabled { get; set; } = false;
      public REFloat SpiritFavor { get; set; } = new(0, -100, 100);
    }
    public Stat Stats { get; protected set; } = new();

    public class SA_WoodCutting : ActionBase {
      #region Overrides
      public override string Name => "벌목";
      public override string Description =>
        $"주변 나무를 베어 재료를 얻는다.\n" +
        $"획득량 : 1\n" +
        $"발생하는 잔가지 수 : {Modifiers.StickGatheringAmount:F2}\n" +
        $"기본 벌목 시간 : {DefaultDuration}s\n" +
        $"탐색 시간 : {Modifiers.ExplorationMinDuration} ~ {Modifiers.ExplorationMaxDuration}s\n" +
        $"숙련도 가중치 : {SkillProficiencyMultiplier:P0}";

      #endregion

      public SA_WoodCutting() {
        DefaultDuration = 5;
        SkillProficiencyMultiplier.Base = 1;
        Repeatable = true;
        Pausable = true;

        RequiredKnowledge = new Keyword[] {
          Keyword.None,
        };
      }
      private float m_explorationDuration;
      public override float Duration => m_explorationDuration + DefaultDuration;

      protected override ActionCallbacks CustomCallbacks => new() {
        OnRepeat = SetExplorationDuration, // Only Completing the action will reset the exploration duration
        OnPerform = () => {
          if (Skill.GetAction<SA_StickGathering>(out var action))
            action.Amount += Modifiers.StickGatheringAmount.Value.PRound();
        },
      };
      private void SetExplorationDuration() {
        m_explorationDuration = UnityEngine.Random.Range(
          Modifiers.ExplorationMinDuration.Value,
          Modifiers.ExplorationMaxDuration.Value
        );
      }

      public record Modifier : ModifierBase {
        public REFloat ExplorationMaxDuration { get; set; } = new(10);
        public REFloat ExplorationMinDuration { get; set; } = new(1);
        public REFloat StickGatheringAmount { get; set; } = new(2);
      }
      [JsonInclude]
      public Modifier Modifiers { get; protected set; } = new();
    }

    public class SA_StickGathering : ActionBase {
      #region Overrides
      public override string Name => "잔가지 회수";
      public override string Description =>
        $"쓰러진 나무에서 잔가지를 회수한다\n" +
        $"회수 가능한 잔가지 수 : {Amount}\n" +
        $"기본 수행 시간 : {DefaultDuration}s\n" +
        $"숙련도 가중치 : {SkillProficiencyMultiplier:P0}";

      public override bool CanPerform => Amount > 0;
      public override string StackInfo => $"×{Amount}";
      #endregion

      [JsonInclude]
      public int Amount { get; set; } = 0;
      public SA_StickGathering() {
        DefaultDuration = 2.5f;
        SkillProficiencyMultiplier.Base = 0.1f;
        Repeatable = true;
      }
      protected override ActionCallbacks CustomCallbacks => new() {
        OnPerform = () => Amount--
      };
    }
  }
}
