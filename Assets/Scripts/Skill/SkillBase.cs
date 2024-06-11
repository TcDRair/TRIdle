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
    public string Name { get; protected set; }
    public int Level { get; protected set; }
    public int Proficiency { get; protected set; }
    public int MaxLevel { get; protected set; }
    protected string IconPath { get; set; }
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
        new ActionBase() { Name = "벌목", Description = "도끼로 나무를 자른다!" }
      };
    }

    public override ProgressType Progress => ProgressType.None;
  }
}