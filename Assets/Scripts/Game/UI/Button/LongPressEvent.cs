using System.Linq;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.UI;

namespace TRIdle.Game.UI
{
  using Logics.Extensions;
  using Entry = EventTrigger.Entry;

  public class LongClickButton : Button
  {
    [SerializeField, Tooltip("Invoked when the button is long pressed and released.")]
    protected UnityEvent m_OnLongClick;
    [SerializeField, Tooltip("Invoked when the button is long pressed.")]
    protected UnityEvent m_OnLongPress;
    [SerializeField, Tooltip("Invoked when the button is pressed and released shortly.")]
    protected UnityEvent m_OnShortClick;
    [SerializeField, Tooltip("Whether to show the progress indicator while long pressing.")]
    private bool m_ShowProgress;
    [SerializeField] private Image m_ProgressIndicator;

    [SerializeField, Tooltip("After this time(seconds) the press is considered as long pressing.")]
    private float m_LongPressThreshold = 0.5f;
    [SerializeField, Tooltip("After this time(seconds) OnLongPress event will be invoked.")]
    private float m_LongPressDuration = 1f;

    public UnityEvent OnLongPress => m_OnLongPress;

    protected override void Awake() {
      base.Awake();
      /*
      // Replace original PointerUp and PointerClick events with our own
      var triggers = m_EventTrigger.triggers;
      Entry pd = triggers.FirstOrDefault(t => t.eventID is EventTriggerType.PointerDown);
      Entry pu = triggers.FirstOrDefault(t => t.eventID is EventTriggerType.PointerUp);
      Entry pc = triggers.FirstOrDefault(t => t.eventID is EventTriggerType.PointerClick);


      // Add PointerDown and PointerUp events to the trigger
      AddListener(m_EventTrigger, EventTriggerType.PointerDown, OnPointerDown);
      AddListener(m_EventTrigger, EventTriggerType.PointerUp, OnQuit);
      AddListener(m_EventTrigger, EventTriggerType.PointerExit, OnQuit);*/
    }

    private float m_pressedTime = float.MaxValue;
    public override void OnPointerClick(PointerEventData eventData) {
      base.OnPointerClick(eventData);
      if(Time.unscaledTime - m_pressedTime < m_LongPressThreshold)
        m_OnShortClick.Invoke();
      else m_OnLongClick.Invoke();
    }
    public override void OnPointerDown(PointerEventData eventData) {
      base.OnPointerDown(eventData);
      m_pressedTime = Time.unscaledTime;
    }
    public override void OnPointerUp(PointerEventData eventData) {
      base.OnPointerUp(eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData) {
      base.OnPointerEnter(eventData);
    }
    public override void OnPointerExit(PointerEventData eventData) {
      base.OnPointerExit(eventData);
    }
  }

  [CustomEditor(typeof(LongClickButton))]
  public class LongPressableButtonEditor : SelectableEditor
  {
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();
    }
  }
}