using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.Skill
{
  using System.Linq;
  using Action;

  [Serializable]
  public abstract class SkillBase
  {
    public string Name { get; protected set; } = Const.PLACEHOLDER;
    public int Level { get; protected set; } = 1;
    public int Proficiency { get; set; } = 1;
    public int MaxLevel { get; protected set; } = -1;
    protected string IconPath { get; set; } = "Sprite/PlaceHolder";
    public Sprite Icon => Resources.Load<Sprite>(IconPath);

    public enum ProgressType
    {
      None,
      /// <summary>Indicates the main panel is focusing this skill. Has lower priority than Tasks.</summary>
      Focused,
      /// <summary>In progress of performing a task.</summary>
      TaskActive,
      /// <summary>Task is completed. Only used for repeatable tasks. Otherwise, use <see cref="Focused"/>.</summary>
      TaskCompleted,
      /// <summary>Task is interrupted by non-player action. Otherwise, use <see cref="Focused"/>.</summary>
      TaskInterrupted
    }
    public abstract ProgressType Progress { get; }

    public ActionBase[] Actions { get; protected set; }
    public ActionBase Current { get; protected set; }
    public T GetAction<T>() where T : ActionBase => Actions.FirstOrDefault(a => a is T) as T;
  }


  // SMP : Sample
  public class SMPSkill_WoodCutting : SkillBase
  {
    public SMPSkill_WoodCutting()
    {
      Name = "벌목";
      Level = 5;
      MaxLevel = 99;
      IconPath = "Sprite/WoodCutting";

      Actions = new ActionBase[] {
        new SMPAction_WoodCutting(this),
        new SMPAction_StickGathering(this)
      };
    }

    public class SMPAction_WoodCutting : ActionBase
    {
      public SMPAction_WoodCutting(SkillBase Skill) : base(Skill)
      {
        Name = "벌목";
        Description = "도끼로 나무를 자른다!";
        Duration = 10;
        Repeatable = true;
        Pausable = true;

        OnPerform = () =>
        {
          Skill.Proficiency += 2;
          if (Skill.GetAction<SMPAction_StickGathering>() is var action)
            action.Amount += 2;
        };
      }
    }

    public class SMPAction_StickGathering : ActionBase
    {
      public int Amount { get; set; } = 0;
      public SMPAction_StickGathering(SkillBase Skill) : base(Skill)
      {
        Name = "잔가지 회수";
        Description = "벌목한 나무의 잔가지를 회수한다!";
        Duration = 4;
        Repeatable = true;

        CanPerform = () => Amount > 0;
        OnPerform = () =>
        {
          Amount--;
          Skill.Proficiency += 2;
        };
      }
    }

    public override ProgressType Progress => ProgressType.None;
  }
}