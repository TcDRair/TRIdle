using System;
using System.Collections;

using UnityEngine;

namespace TRIdle.Game.PlayerInternal
{
  using UI;
  using Skill;
  using Logics.Extensions;

  /// <summary>
  /// PlayerMono is a MonoBehaviour that provides delay and cooldown functionalities.
  /// Controls time-based actions with coroutines.
  /// </summary>
  public class PlayerMono : MonoBehaviour
  {
    private PlayerData Data => Player.Instance.Data;

    public float DelayDuration, DelayElapsed;

    private ActionBase m_delayingAction;
    private Coroutine m_delayCoroutine;

    public void StartActionDelay(ActionBase action) {
      // same action : ignore
      if (action == m_delayingAction) return;
      // other action : stop current action (delay)
      if (m_delayCoroutine is not null) {
        StopCoroutine(m_delayCoroutine);
        if (m_delayingAction is not null) m_delayingAction.Progress = 0;
      }
      // null action : update and ignore
      if ((m_delayingAction = action) is null) return;
      // start delay
      this.Log("Start Delay!");
      m_delayCoroutine = StartCoroutine(DelayLoop());
    }

    IEnumerator DelayLoop() {
      while (m_delayingAction is not null) {
        DelayElapsed = 0;
        DelayDuration = m_delayingAction.Data.Duration.Value;

        while (DelayElapsed < DelayDuration) {
          DelayElapsed += Time.deltaTime;
          m_delayingAction.Progress = DelayElapsed / DelayDuration;
          yield return null;
        }

        m_delayingAction.Activate();
        m_delayingAction.Progress = DelayElapsed = 0;
        UI_MainSceneController.Instance.Menu_Update();
      }
    }
  }
}