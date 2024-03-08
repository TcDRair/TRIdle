using System;

namespace TRIdle.Game
{
  public partial class Storage
  {
    public struct R_Professions
    {
      public P_StockManagement CommodityManagement;
    }
    public R_Professions Professions;
  }

  public abstract class Profession
  {
    /// <summary>이 전문기술의 이름을 나타냅니다.</summary>
    public abstract string Name { get; }
    /// <summary>이 전문기술의 주요 설명입니다.</summary>
    public abstract string Description { get; }
    public abstract MainProfession MainProfession { get; }
    /// <summary>이 전문기술의 최대 등급을 나타냅니다.</summary>
    public abstract int MaxGrade { get; }
    /// <summary>이 전문기술의 등급별 최대 레벨을 나타냅니다.</summary>
    public abstract int[] MaxLevel { get; }
    /// <summary>이 전문기술의 주 등급을 나타냅니다.</summary>
    public int Grade { get; protected set; } = 1;
    /// <summary>이 전문기술의 현재 등급 내 레벨을 나타냅니다.</summary>
    public int Level { get; protected set; } = 0;
    /// <summary>이 전문기술의 등급 내 숙련도를 나타냅니다.</summary>
    public long Proficiency { get; protected set; } = 0;

    /// <summary>이 전문기술의 숙련도를 상승시킵니다. 결과에 따라 레벨이나 등급이 상승할 수 있습니다.</summary>
    public virtual void AddProficiency(long amount) {
      // Check if the proficiency is enough to level up
      if ((Proficiency += amount) < RequiredProficiency(Grade, Level)) return;
      // Level up
      if (Level++ <= MaxLevel[Grade]) return;
      if (Grade < MaxGrade) { Grade++; Level = 0; }
    }
    /// <summary>다음 레벨까지 필요한 숙련도 수치를 나타냅니다.</summary>
    public abstract long RequiredProficiency(int grade, int level);

    /// <summary>이 전문기술의 등급, 레벨, 숙련도를 검증합니다.</summary>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected void Validate(int grade, int level, int proficiency) {
      if (MaxLevel.Length != MaxGrade - 1)
        throw new InvalidOperationException($"The length of {nameof(MaxLevel)}({MaxLevel.Length}) is invalid({MaxGrade - 1}).");
      if (RequiredProficiency(grade, level) > proficiency)
        throw new ArgumentException($"The proficiency({proficiency}) is not enough to reach the level({RequiredProficiency(grade, level)}).");
      if (RequiredProficiency(grade, level + 1) <= proficiency)
        throw new ArgumentException($"The proficiency({proficiency}) is too much for the level({RequiredProficiency(grade, level) - 1}).");
      if (grade < 0 || grade > MaxGrade)
        throw new ArgumentOutOfRangeException($"The grade({grade}) is out of range({MaxGrade}).");
    }
    /// <summary>현재 등급 및 레벨에 따른 지속 효과를 계산합니다. 최초 한 번만 사용됩니다.</summary>
    protected abstract void LoadPassives();
    /// <summary>해당 등급 달성 시 발생하는 지속 효과를 적용합니다. 등급 상승 시 사용됩니다.</summary>
    protected abstract void ApplyPassives(int grade);
    /// <summary>해당 레벨 달성 시 발생하는 지속 효과를 적용합니다. 레벨 상승 시 사용됩니다.</summary>
    protected abstract void ApplyPassives(int grade, int level);
  }
  public enum MainProfession {
    Management, Survival, Gathering, Crafting, Combat
  }
  public class P_StockManagement : Profession
  {
    public override string Name => "물품 관리";
    public override string Description => "물품을 효율적으로 관리하는 기술입니다.";
    public override MainProfession MainProfession => MainProfession.Management;
    public override int MaxGrade => 10;
    public override int[] MaxLevel => new int[] { 20, 25, 30, 35, 40, 45, 50, 50, 50 };

    #region Values
    /// <summary>재고 관리 수치를 나타냅니다.</summary>
    public int StockChecking { get; protected set; } = 10;
    /// <summary>공간 지각 수치를 나타냅니다.</summary>
    public int SpatialPerception { get; protected set; } = 10;

    /// <summary>재고 파악 작업의 수행 시간을 조정합니다.</summary>
    public float StocktakingTimeModifier { get; protected set; } = 1f;
    /// <summary>재고 파악 작업의 지속 시간을 조정합니다.</summary>
    public float StocktakingDurationModifier { get; protected set; } = 1f;
    /// <summary>최적화 작업의 수행 시간을 조정합니다.</summary>
    public float UtilizationTimeModifier { get; protected set; } = 1f;
    /// <summary>최적화 작업의 지속 시간을 조정합니다.</summary>
    public float UtilizationDurationModifier { get; protected set; } = 1f;
    /// <summary>확장 작업의 수행 시간을 조정합니다.</summary>
    public float ExpansionTimeModifier { get; protected set; } = 1f;
    /// <summary>개선 작업의 소모 자원 비율을 조정합니다.</summary>
    public float ImprovementResourceModifier { get; protected set; } = 1f;
    /// <summary>진행 중단이 발생할 확률을 조정합니다.</summary>
    public float InterruptionProbabilityModifier { get; protected set; } = 1f;

    /// <summary>권장 조건 내 숙련도 보너스를 조정합니다.</summary>
    public float RecommendedProficiencyModifier { get; protected set; } = 1f;
    /// <summary>각 등급별 숙련도 보너스를 조정합니다.</summary>
    public float ProficiencyModifier_Tier1 { get; protected set; } = 1f; // 기초
    public float ProficiencyModifier_Tier2 { get; protected set; } = 1f; // 초급
    public float ProficiencyModifier_Tier3 { get; protected set; } = 1f; // 중급
    public float ProficiencyModifier_Tier4 { get; protected set; } = 1f; // 고급
    public float ProficiencyModifier_Tier5 { get; protected set; } = 1f; // 상급
    public float ProficiencyModifier_Tier6 { get; protected set; } = 1f; // 특급
    #endregion

    public P_StockManagement(int grade, int level, int proficiency) {
      Grade = grade;
      Level = level;
      Proficiency = proficiency;
      Validate(grade, level, proficiency);
    }

    public override long RequiredProficiency(int grade, int level)
      => (long)(Math.Pow(level, 1.2) * Math.Pow(1.15, 5 * (grade + Math.Min(grade + 3, 10))) / 3);

    protected override void LoadPassives() {
      // Reset
      StockChecking = SpatialPerception = 10;
      StocktakingTimeModifier = StocktakingDurationModifier = 1f;
      UtilizationTimeModifier = UtilizationDurationModifier = 1f;
      ProficiencyModifier_Tier1 = ProficiencyModifier_Tier2 = ProficiencyModifier_Tier3 = 1f;
      ProficiencyModifier_Tier4 = ProficiencyModifier_Tier5 = ProficiencyModifier_Tier6 = 1f;
      RecommendedProficiencyModifier = 1f;
      ExpansionTimeModifier = ImprovementResourceModifier = InterruptionProbabilityModifier = 1f;
      // Apply
      for (int g = 1; g < Grade; g++) {
        ApplyPassives(g);
        for (int l = 0; l < Level; l++)
          ApplyPassives(g, l);
      }
    }
    protected override void ApplyPassives(int grade) {
      switch (grade) {
        //TODO 각 수치 및 작업 구현
        case 1:
          // 기초 물품 관리 건물 해금
          ProficiencyModifier_Tier1 = 4f;
          break;
        case 2:
          ProficiencyModifier_Tier1 = 1f;
          break;
        case 3:
          // 초급 물품 관리 건물 해금
          ProficiencyModifier_Tier2 = 2.5f;
          break;
        case 4:
          ProficiencyModifier_Tier2 = 1f;
          break;
        case 5:
          // 중급 물품 관리 건물 해금
          ProficiencyModifier_Tier3 = 1.75f;
          break;
        case 6:
          // 저장고 확장 작업 해금
          ProficiencyModifier_Tier3 = 1f;
          break;
        case 7:
          // 고급 물품 관리 건물 해금
          ProficiencyModifier_Tier4 = 1.5f;
          break;
        case 8:
          // 집적도 개선 작업 해금
          ProficiencyModifier_Tier4 = 1f;
          break;
        case 9:
          // 상급 물품 관리 건물 해금
          ProficiencyModifier_Tier5 = 1.25f;
          break;
        case 10:
          ProficiencyModifier_Tier5 = 1f;
          break;
      }
    }
    protected override void ApplyPassives(int grade, int level) {
      StockChecking += grade;
      SpatialPerception += grade;
      switch (grade) {
        case 2:
          RecommendedProficiencyModifier = 1 + level / MaxLevel[grade];
          break;
        case 3:
          if (level % 10 == 0) {
            StockChecking += 4;
            SpatialPerception += 4;
          }
          break;
        case 4:
          InterruptionProbabilityModifier = .9f * (1 - level / MaxLevel[grade]);
          break;
        case 5:
          if (level % 5 == 0) {
            StockChecking += 4;
            SpatialPerception += 4;
          }
          break;
        case 6:
          ExpansionTimeModifier = 1 - .75f * level / MaxLevel[grade];
          break;
        case 7:
          if (level % 2 == 0) {
            StockChecking += 4;
            SpatialPerception += 4;
          }
          break;
        case 8:
          ImprovementResourceModifier = 1 - .5f * level / MaxLevel[grade];
          break;
        case 10:
          if (level <= 80) {
            StocktakingTimeModifier -= 0.01f;
            UtilizationTimeModifier -= 0.01f;
            StocktakingDurationModifier += 0.05f;
            UtilizationDurationModifier += 0.05f;
          }
          break;
      }
    }
  }


}
