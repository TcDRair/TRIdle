using System;
using UnityEngine;

namespace TRIdle.Game
{
  using Skill;
  
  [CreateAssetMenu(fileName = "ProgressButton_Ref", menuName = "TRIdle/ProgressButton References", order = 0)]
  public class ProgressButton_Ref : ScriptableObject
  {
    [Serializable] public class Icon
    {
      public Sprite ActionCompleted, ActionInturrupted, ActionAwaiting;
    }
    public Icon icon;
  }
}