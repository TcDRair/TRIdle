using UnityEngine;

namespace TRIdle.Game.Skill.Action
{
  public class ActionBase
  {
    public string Name { get; set; } = Const.PLACEHOLDER;
    public string Description { get; set; } = Const.PLACEHOLDER;

    public float Duration { get; set; } = 4;
    public bool Repeatable { get; set; } = true;

    public System.Action OnPerform { get; set; }
  }
}