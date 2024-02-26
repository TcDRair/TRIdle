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
    /// <summary>�� ��������� �̸��� ��Ÿ���ϴ�.</summary>
    public abstract string Name { get; }
    /// <summary>�� ��������� �ֿ� �����Դϴ�.</summary>
    public abstract string Description { get; }
    /// <summary>�� ��������� �ִ� ����� ��Ÿ���ϴ�.</summary>
    public abstract int MaxGrade { get; }
    /// <summary>�� ��������� ��޺� �ִ� ������ ��Ÿ���ϴ�.</summary>
    public abstract int[] MaxLevel { get; }
    /// <summary>�� ��������� �� ����� ��Ÿ���ϴ�.</summary>
    public int Grade { get; protected set; } = 1;
    /// <summary>�� ��������� ���� ��� �� ������ ��Ÿ���ϴ�.</summary>
    public int Level { get; protected set; } = 0;
    /// <summary>�� ��������� ��� �� ���õ��� ��Ÿ���ϴ�.</summary>
    public long Proficiency { get; protected set; } = 0;

    /// <summary>�� ��������� ���õ��� ��½�ŵ�ϴ�. ����� ���� �����̳� ����� ����� �� �ֽ��ϴ�.</summary>
    public virtual void AddProficiency(long amount) {
      // Check if the proficiency is enough to level up
      if ((Proficiency += amount) < RequiredProficiency(Grade, Level)) return;
      // Level up
      if (Level++ <= MaxLevel[Grade]) return;
      if (Grade < MaxGrade) { Grade++; Level = 0; }
    }
    /// <summary>���� �������� �ʿ��� ���õ� ��ġ�� ��Ÿ���ϴ�.</summary>
    public abstract long RequiredProficiency(int grade, int level);

    /// <summary>�� ��������� ���, ����, ���õ��� �����մϴ�.</summary>
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
    /// <summary>���� ��� �� ������ ���� ���� ȿ���� ����մϴ�. ���� �� ���� ���˴ϴ�.</summary>
    protected abstract void LoadPassives();
    /// <summary>�ش� ��� �޼� �� �߻��ϴ� ���� ȿ���� �����մϴ�. ��� ��� �� ���˴ϴ�.</summary>
    protected abstract void ApplyPassives(int grade);
    /// <summary>�ش� ���� �޼� �� �߻��ϴ� ���� ȿ���� �����մϴ�. ���� ��� �� ���˴ϴ�.</summary>
    protected abstract void ApplyPassives(int grade, int level);
  }
  public class P_StockManagement : Profession
  {
    public override string Name => "��ǰ ����";
    public override string Description => "��ǰ�� ȿ�������� �����ϴ� ����Դϴ�.";
    public override int MaxGrade => 10;
    public override int[] MaxLevel => new int[] { 20, 25, 30, 35, 40, 45, 50, 50, 50 };

    #region Values
    /// <summary>��� ���� ��ġ�� ��Ÿ���ϴ�.</summary>
    public int StockChecking { get; protected set; } = 10;
    /// <summary>���� ���� ��ġ�� ��Ÿ���ϴ�.</summary>
    public int SpatialPerception { get; protected set; } = 10;

    /// <summary>��� �ľ� �۾��� ���� �ð��� �����մϴ�.</summary>
    public float StocktakingTimeModifier { get; protected set; } = 1f;
    /// <summary>��� �ľ� �۾��� ���� �ð��� �����մϴ�.</summary>
    public float StocktakingDurationModifier { get; protected set; } = 1f;
    /// <summary>����ȭ �۾��� ���� �ð��� �����մϴ�.</summary>
    public float UtilizationTimeModifier { get; protected set; } = 1f;
    /// <summary>����ȭ �۾��� ���� �ð��� �����մϴ�.</summary>
    public float UtilizationDurationModifier { get; protected set; } = 1f;
    /// <summary>Ȯ�� �۾��� ���� �ð��� �����մϴ�.</summary>
    public float ExpansionTimeModifier { get; protected set; } = 1f;
    /// <summary>���� �۾��� �Ҹ� �ڿ� ������ �����մϴ�.</summary>
    public float ImprovementResourceModifier { get; protected set; } = 1f;
    /// <summary>���� �ߴ��� �߻��� Ȯ���� �����մϴ�.</summary>
    public float InterruptionProbabilityModifier { get; protected set; } = 1f;

    /// <summary>���� ���� �� ���õ� ���ʽ��� �����մϴ�.</summary>
    public float RecommendedProficiencyModifier { get; protected set; } = 1f;
    /// <summary>�� ��޺� ���õ� ���ʽ��� �����մϴ�.</summary>
    public float ProficiencyModifier_Tier1 { get; protected set; } = 1f; // ����
    public float ProficiencyModifier_Tier2 { get; protected set; } = 1f; // �ʱ�
    public float ProficiencyModifier_Tier3 { get; protected set; } = 1f; // �߱�
    public float ProficiencyModifier_Tier4 { get; protected set; } = 1f; // ���
    public float ProficiencyModifier_Tier5 { get; protected set; } = 1f; // ���
    public float ProficiencyModifier_Tier6 { get; protected set; } = 1f; // Ư��
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
        //TODO �� ��ġ �� �۾� ����
        case 1:
          // ���� ��ǰ ���� �ǹ� �ر�
          ProficiencyModifier_Tier1 = 4f;
          break;
        case 2:
          ProficiencyModifier_Tier1 = 1f;
          break;
        case 3:
          // �ʱ� ��ǰ ���� �ǹ� �ر�
          ProficiencyModifier_Tier2 = 2.5f;
          break;
        case 4:
          ProficiencyModifier_Tier2 = 1f;
          break;
        case 5:
          // �߱� ��ǰ ���� �ǹ� �ر�
          ProficiencyModifier_Tier3 = 1.75f;
          break;
        case 6:
          // ����� Ȯ�� �۾� �ر�
          ProficiencyModifier_Tier3 = 1f;
          break;
        case 7:
          // ��� ��ǰ ���� �ǹ� �ر�
          ProficiencyModifier_Tier4 = 1.5f;
          break;
        case 8:
          // ������ ���� �۾� �ر�
          ProficiencyModifier_Tier4 = 1f;
          break;
        case 9:
          // ��� ��ǰ ���� �ǹ� �ر�
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