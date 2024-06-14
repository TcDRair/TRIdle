using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TRIdle.Game.Skill
{
  [Serializable]
  public abstract class SkillBase
  {
    public string Name { get; protected set; }
    public int Level { get; protected set; }
    public int MaxLevel { get; protected set; }

    public enum ProgressType
    {
      None,
      Focused,
      TaskOngoing,
      TaskCompleted,
      TaskAwaiting,
    }
    public abstract ProgressType Progress { get; }
  }


  public class SMPSkill_WoodCutting : SkillBase
  {
    public SMPSkill_WoodCutting() {
      Name = "벌목";
      Level = 5;
      MaxLevel = 99;
    }

    public override ProgressType Progress => ProgressType.None;
  }
}