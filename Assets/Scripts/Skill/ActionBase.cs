using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace TRIdle.Game.Skill.Action
{
  [Serializable]
  [JsonDerivedType(typeof(SMPSkill_WoodCutting.SMPAction_WoodCutting))]
  [JsonDerivedType(typeof(SMPSkill_WoodCutting.SMPAction_StickGathering))]
  public class ActionBase
  {
    public string Name { get; set; } = Const.PLACEHOLDER;
    public string Description { get; set; } = Const.PLACEHOLDER;

    public float Duration { get; set; } = 4;
    public bool Repeatable { get; set; } = true;
    public bool Pausable { get; set; } = false;

    [JsonIgnore]
    public SkillBase Skill { get; private set; }
    public ActionBase(SkillBase skill)
    {
      Skill = skill;
    }

    [JsonIgnore]
    public Func<bool> CanPerform { get; set; } = () => true;
    [JsonIgnore]
    public Func<string> StackInfo { get; set; } = () => "";
    [JsonIgnore]
    public System.Action OnPerform { get; set; } = () => { };
  }
}