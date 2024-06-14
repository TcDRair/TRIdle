using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.Skill
{
  using Action;


  [Serializable]
  public abstract class SkillBase
  {
    public string Name { get; protected set; } = Const.PLACEHOLDER;
    public int Level { get; protected set; } = 1;
    public int Proficiency { get; protected set; } = 1;
    public int MaxLevel { get; protected set; } = -1;
    protected string IconPath { get; set; } = "Sprite/PlaceHolder";
    public Sprite Icon => Resources.Load<Sprite>(IconPath);

    public enum ProgressType
    {
      None,
      Focused,
      TaskOngoing,
      TaskAwaiting,
      TaskCompleted,
    }
    public abstract ProgressType Progress { get; }

    public ActionBase[] Actions { get; protected set; }
  }

  public class SMPSkill_WoodCutting : SkillBase
  {
    public SMPSkill_WoodCutting() {
      Name = "벌목";
      Level = 5;
      MaxLevel = 99;
      IconPath = "Sprite/WoodCutting";

      Actions = new[] {
        new ActionBase() {
          Name = "벌목",
          Description = "도끼로 나무를 자른다!",
          Duration = 4,
          Repeatable = true,

          OnPerform = () => {
            Proficiency += 1;
          }
        }
      };
    }

    public override ProgressType Progress => ProgressType.None;
  }
}