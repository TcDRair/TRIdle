using UnityEngine;

namespace TRIdle.Game.Skill
{
  using System.Collections.Generic;
  using Logics.Math;

  public sealed class Effect_Common_Internalization : EffectBase<Effect_Common_Internalization>
  {
    public override string Name => Text.Common.Effect_Internalization_Name;
    public override string Description => Text.Common.Effect_Internalization_Description;
    public override SkillBase Category => Skill_Common.Instance;

    private readonly Dictionary<ActionBase, int> m_cumulated = new();

    private ActionBase Current => Player.Instance.Data.CurrentAction;
    public override void OnUpdate() {
      // Add current action if not in the dictionary
      if (m_cumulated.TryGetValue(Current, out _) is false) {
        m_cumulated.Add(Current, 0);
        Current.Data.Speed.Modifiers += ModifyActionSpeed(Current);
      }

      // Update stack count
      foreach (var action in m_cumulated.Keys) {
        if (action == Current) m_cumulated[action]++;
        else if (--m_cumulated[action] == 0) {
          action.Data.Speed.Modifiers -= ModifyActionSpeed(action);
          m_cumulated.Remove(action);
        }
      }
    }

    private const int Yield = 30, Duration = 1800;
    private const float SpeedMultiplier = 0.2f;
    Modifier ModifyActionSpeed(ActionBase action) 
      => m_cumulated.TryGetValue(action, out int count) ?
        value => {
          value.multiplier += Mathf.Clamp(count - Yield, 0f, Duration) / Duration * SpeedMultiplier;
          return value;
        } :
        value => value; // Somehow if the action is not in the dictionary, it should not be modified

    public override void OnActivate() { }
    public override void OnDeactivate() { }
  }
}