// #undef UNITY_EDITOR // Debug
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
  using Logics.Attributes;

  public class LongClickButton : Button
  {
    #region Serialized Fields
    [SerializeField] protected UnityEvent m_OnLongClick, m_OnLongPress, m_OnShortClick, m_OnAnyPress;

    [SerializeField, Tooltip("Whether to show the progress indicator while long pressing.")]
    private bool m_ShowProgress;
    [SerializeField] private Image m_ProgressIndicator;

    [SerializeField, Tooltip("After this time(seconds) the press is considered as long pressing.")]
    private float m_LongClickThreshold = 0.5f;
    [SerializeField, Tooltip("After this time(seconds) OnLongPress event will be invoked.")]
    private float m_LongPressDuration = 1f;

    [SerializeField] private bool m_EnableLongClick = true;
    private enum PressState { Free, Press, LongPress, LongEnoughPress }
    [SerializeField, ReadonlyField] private PressState m_State;
#if UNITY_EDITOR
    [SerializeField, HideInInspector] private bool m_Initialized = false, m_Foldout1, m_Foldout2;
#endif
    #endregion

#if UNITY_EDITOR
    protected override void OnValidate() {
      base.OnValidate();
      if (m_Initialized is false && m_ShowProgress && Application.isPlaying is false) {
        // Create default progress indicator if it's not set.
        if (m_ProgressIndicator == null) {
          var go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/Long Click Button - Progress Indicator.prefab");
          m_ProgressIndicator = Instantiate(go, transform).GetComponent<Image>();
          this.Log("Default progress indicator created. Please check it in inspector");
        }
        m_Initialized = true;
      }
    }
#endif

    private bool IndicatorEnabled => m_ShowProgress && m_ProgressIndicator != null;
    private float m_pressedTime = float.MaxValue;
    private Coroutine m_pointerDownCoroutine;
    public override void OnPointerClick(PointerEventData eventData) {
      if (m_EnableLongClick) {
        if (m_State is PressState.Press) m_OnShortClick?.Invoke();
        else if (m_State > PressState.Press) m_OnLongClick?.Invoke();
      }

      base.OnPointerClick(eventData);
      m_State = PressState.Free;
    }
    public override void OnPointerDown(PointerEventData eventData) {
      // Process default behaviour
      base.OnPointerDown(eventData);
      if (m_EnableLongClick) m_OnAnyPress?.Invoke();

      if (m_pointerDownCoroutine is not null) TryAbortLongPress();

      m_pressedTime = Time.unscaledTime;
      if (IndicatorEnabled) {
        m_ProgressIndicator.gameObject.SetActive(true);
        m_ProgressIndicator.rectTransform.position = eventData.position;
      }
      m_pointerDownCoroutine = StartCoroutine(PressDownEnumerator());
    }
    public override void OnPointerUp(PointerEventData eventData) {
      base.OnPointerUp(eventData);

      if (m_EnableLongClick) TryAbortLongPress();
    }
    public override void OnPointerExit(PointerEventData eventData) {
      base.OnPointerExit(eventData);

      if (m_EnableLongClick) TryAbortLongPress();
    }

    private IEnumerator PressDownEnumerator() {
      m_State = PressState.Press;
      m_pressedTime = 0;
      while (m_pressedTime < m_LongPressDuration) {
        if (IndicatorEnabled) {
          var color = m_ProgressIndicator.color;
          var lerp = Mathf.Clamp01((m_pressedTime - m_LongClickThreshold) / (m_LongPressDuration - m_LongClickThreshold));
          color.a = lerp;
          m_ProgressIndicator.color = color;
          m_ProgressIndicator.fillAmount = m_pressedTime / m_LongPressDuration;
        }
        if (m_State is PressState.Press && m_pressedTime >= m_LongClickThreshold)
          m_State = PressState.LongPress;
        yield return null;
        m_pressedTime += Time.deltaTime;
      }
      m_OnLongPress?.Invoke();
      ResetIndicator();
      m_State = PressState.LongEnoughPress;
    }
    private void TryAbortLongPress() {
      if (m_pointerDownCoroutine is not null) {
        StopCoroutine(m_pointerDownCoroutine);
        m_pointerDownCoroutine = null;
      }
      if (IndicatorEnabled) ResetIndicator();
    }
    private void ResetIndicator() {
      m_ProgressIndicator.fillAmount = 0f;
      m_ProgressIndicator.gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LongClickButton))]
    public class LongPressableButtonEditor : SelectableEditor
    {
      private GUIStyle BoldFoldoutStyle => new(EditorStyles.foldout) { fontStyle = FontStyle.Bold };

      public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        // Main Switch
        var enableProperty = serializedObject.FindProperty(nameof(m_EnableLongClick));
        EditorGUILayout.PropertyField(enableProperty);
        var threshold = serializedObject.FindProperty(nameof(m_LongClickThreshold));
        EditorGUILayout.Space();

        var foldout1 = serializedObject.FindProperty(nameof(m_Foldout1));
        var foldout2 = serializedObject.FindProperty(nameof(m_Foldout2));

        if (enableProperty.boolValue) {
          EditorGUI.indentLevel++;

          var duration = serializedObject.FindProperty(nameof(m_LongPressDuration));
          threshold.floatValue = EditorGUILayout.Slider(threshold.displayName, threshold.floatValue, .01f, duration.floatValue);
          duration.floatValue = EditorGUILayout.Slider(duration.displayName, duration.floatValue, threshold.floatValue, 5);

          EditorGUI.indentLevel--;

          if (foldout1.boolValue = EditorGUILayout.Foldout(foldout1.boolValue, "Long Click Properties", true, BoldFoldoutStyle)) {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(m_State)));
            var showProgressProperty = serializedObject.FindProperty(nameof(m_ShowProgress));
            EditorGUILayout.PropertyField(showProgressProperty);
            if (showProgressProperty.boolValue) {
              var progressIndicatorProperty = serializedObject.FindProperty(nameof(m_ProgressIndicator));
              EditorGUILayout.PropertyField(progressIndicatorProperty);
              if (progressIndicatorProperty.objectReferenceValue == null)
                EditorGUILayout.HelpBox("Need to set a progress indicator to show the progress", MessageType.Warning);
              else if ((progressIndicatorProperty.objectReferenceValue as Image).type is not Image.Type.Filled)
                EditorGUILayout.HelpBox("Progress indicator must be of type Filled Image.", MessageType.Warning);
            }

            EditorGUI.indentLevel--;
          }
          EditorGUILayout.Space();

          // Events
          if (foldout2.boolValue = EditorGUILayout.Foldout(foldout2.boolValue, "Events", true, BoldFoldoutStyle)) {
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField(new GUIContent() { text = "Long Click Event", tooltip = "Invoked when the button is pressed for a long time and released." }, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(m_OnLongClick)));
            EditorGUILayout.LabelField(new GUIContent() { text = "Long Press Event", tooltip = "Invoked when the button is pressed long enough." }, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(m_OnLongPress)));
            EditorGUILayout.LabelField(new GUIContent() { text = "Short Click Event", tooltip = "Invoked when the button is pressed and released shortly." }, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(m_OnShortClick)));
            EditorGUILayout.LabelField(new GUIContent() { text = "Press Event", tooltip = "Invoked instantly when the button is pressed." }, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(m_OnAnyPress)));

            EditorGUI.indentLevel--;
          }
          serializedObject.ApplyModifiedProperties();
        }
        else
          EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnClick"));
        serializedObject.ApplyModifiedProperties();
      }
    }
#endif
  }
}