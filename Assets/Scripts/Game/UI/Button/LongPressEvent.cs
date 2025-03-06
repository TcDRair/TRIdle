using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TRIdle.Game.UI
{
  using Logics.Extensions;
  using Entry = EventTrigger.Entry;

  [RequireComponent(typeof(Image))]
  public class LongPressEvent : MonoBehaviour
  {
    [SerializeField] private EventTrigger m_EventTrigger;
    public UnityEvent OnLongPress;
    [SerializeField] private Image m_ProgressIndicator;
    [SerializeField] private bool m_ShowProgress;

    private const float LongPressThreshold = 0.25f;
    private const float LongPressDuration = 1f;
    private float m_elapsedTime;
    private Coroutine m_coroutine;

    private void Awake() {
      // Check if the trigger exists, otherwise create it
      if (m_EventTrigger == null && transform.TryGetComponentInParent(out m_EventTrigger)) {
        this.LogWarning("No EventTrigger found on this element. Disabled after this message.");
        enabled = false;
        return;
      }

      // Add PointerDown and PointerUp events to the trigger
      AddListener(m_EventTrigger, EventTriggerType.PointerDown, OnPointerDown);
      AddListener(m_EventTrigger, EventTriggerType.PointerUp, OnQuit);
      AddListener(m_EventTrigger, EventTriggerType.PointerExit, OnQuit);
    }

    private void AddListener(EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> action) {
      if (trigger.triggers.Find(e => e.eventID == type) is Entry entry)
        entry.callback.AddListener(action);
      else {
        Entry newEntry = new() { eventID = type };
        newEntry.callback.AddListener(action);
        trigger.triggers.Add(newEntry);
      }
    }

    private void OnPointerDown(BaseEventData eventData) {
      if (m_coroutine is not null) return; // Don't allow multiple long presses at once
      m_ProgressIndicator.rectTransform.position = (eventData as PointerEventData).position;
      m_coroutine = StartCoroutine(InvokeAfterDelay());
    }

    private bool m_indicatorTrigger = false;
    IEnumerator InvokeAfterDelay() {
      m_elapsedTime = 0;

      if (m_ShowProgress) {
        m_ProgressIndicator.enabled = true;
        m_ProgressIndicator.CrossFadeAlpha(0, 0, true);
      }

      while (m_elapsedTime < LongPressDuration) {
        m_elapsedTime += Time.unscaledDeltaTime;
        m_ProgressIndicator.fillAmount = m_elapsedTime / LongPressDuration;

        if (m_ShowProgress && m_indicatorTrigger is false && m_elapsedTime >= LongPressThreshold) {
          m_ProgressIndicator.CrossFadeAlpha(1, LongPressDuration - LongPressThreshold, true);
          m_indicatorTrigger = true;
        }

        yield return null; // Wait until the specified time has elapsed
      }
      
      OnLongPress.Invoke();
      m_indicatorTrigger = false;
    }

    private void OnQuit(BaseEventData eventData) {
      if (m_coroutine is not null)
        StopCoroutine(m_coroutine);
      
      m_coroutine = null;

      if (m_ShowProgress) {
        m_indicatorTrigger = false;
        m_ProgressIndicator.CrossFadeAlpha(0, 0, true);
        m_ProgressIndicator.enabled = false;
      }
    }
  }
}