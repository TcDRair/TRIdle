using System.Collections;

using UnityEngine;

namespace TRIdle.Game.PlayerInternal
{
  public class PlayerMono : MonoBehaviour
  {
    public float DelayDuration, DelayRemaining;
    public float CooldownDuration, CooldownRemaining;
    public Coroutine DelayCoroutine, CooldownCoroutine;

    public void StartDelay(float delay, System.Action action) {
      if (DelayCoroutine is not null)
        StopCoroutine(DelayCoroutine);
      DelayDuration = delay;
      DelayCoroutine = StartCoroutine(Delay(action));

      IEnumerator Delay(System.Action action) {
        DelayRemaining = DelayDuration;
        while (DelayRemaining > 0) {
          DelayRemaining -= Time.deltaTime;
          yield return null;
        }
        action?.Invoke();
      }
    }

    public void StartCooldown(float cooldown, System.Action action) {
      if (CooldownCoroutine is not null)
        StopCoroutine(CooldownCoroutine);
      CooldownDuration = cooldown;
      CooldownCoroutine = StartCoroutine(Cooldown(action));

      IEnumerator Cooldown(System.Action action) {
        CooldownRemaining = CooldownDuration;
        while (CooldownRemaining > 0) {
          CooldownRemaining -= Time.deltaTime;
          yield return null;
        }
        action?.Invoke();
      }
    }
  }
}