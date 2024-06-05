using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRIdle.Game.Action
{
  public class ActionBase
  {
    public float Proficiency { get; protected set; }
    public float Progress { get; protected set; }

    [SerializeField, InspectorName("Button Prefab")] GameObject m_RButtonPrefab;


  }
}