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
  public class LongClickButton : Button
  {
    #region Serialized Fields
    [SerializeField] protected UnityEvent m_OnLongClick, m_OnLongPress, m_OnShortClick;

    [SerializeField, Tooltip("Whether to show the progress indicator while long pressing.")]
    private bool m_ShowProgress;
    [SerializeField] private Image m_ProgressIndicator;

    [SerializeField, Tooltip("After this time(seconds) the press is considered as long pressing.")]
    private float m_LongPressThreshold = 0.5f;
    [SerializeField, Tooltip("After this time(seconds) OnLongPress event will be invoked.")]
    private float m_LongPressDuration = 1f;

    [SerializeField] private bool m_EnableLongClick = true;
#if UNITY_EDITOR
    [SerializeField, HideInInspector] private bool m_Initialized = false, m_Foldout1, m_Foldout2;
#endif
    #endregion

    public UnityEvent OnLongPress => m_OnLongPress;

#if UNITY_EDITOR
    protected override void OnValidate() {
      base.OnValidate();
      if (m_Initialized is false && m_ShowProgress && Application.isPlaying is false) {
        // Create default progress indicator if it's not set.
        if (m_ProgressIndicator == null) {
          var go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/Long Click Button - Progress Indicator.prefab");
          m_ProgressIndicator = Instantiate(go, transform).GetComponent<Image>();
        }
        this.Log("Default progress indicator created. Please check it in inspector");
        m_Initialized = true;
      }
    }
#endif

    private float m_pressedTime = float.MaxValue;
    private Coroutine m_pointerDownCoroutine;
    public override void OnPointerClick(PointerEventData eventData) {
      base.OnPointerClick(eventData);

      if (m_EnableLongClick is false) return;
      else if (Time.unscaledTime - m_pressedTime < m_LongPressThreshold) m_OnShortClick?.Invoke();
      else m_OnLongClick?.Invoke();
    }
    public override void OnPointerDown(PointerEventData eventData) {
      base.OnPointerDown(eventData);
      if (m_EnableLongClick is false || m_pointerDownCoroutine is null) return;

      m_pressedTime = Time.unscaledTime;
      if (m_ShowProgress && m_ProgressIndicator != null)
        m_ProgressIndicator.rectTransform.position = eventData.position;
      m_pointerDownCoroutine = StartCoroutine(PressDownEnumerator());
    }
    public override void OnPointerUp(PointerEventData eventData) {
      base.OnPointerUp(eventData);
      AbortLongPress();
    }
    public override void OnPointerExit(PointerEventData eventData) {
      base.OnPointerExit(eventData);
      AbortLongPress();
    }

    private IEnumerator PressDownEnumerator() {
      float elapsed;
      while ((elapsed = Time.unscaledTime - m_pressedTime) < m_LongPressDuration) {
        if (m_ShowProgress && m_ProgressIndicator != null) {
          var color = m_ProgressIndicator.color;
          var lerp = Mathf.Clamp01((elapsed - m_LongPressThreshold) / (m_LongPressDuration - m_LongPressThreshold));
          color.a = lerp;
          m_ProgressIndicator.color = color;
          m_ProgressIndicator.fillAmount = (Time.unscaledTime - m_pressedTime) / m_LongPressDuration;
        }
        yield return null;
      }
      m_OnLongPress?.Invoke();
    }
    private void AbortLongPress() {
      if (m_EnableLongClick is false) return;

      m_pressedTime = float.MaxValue;
      if (m_pointerDownCoroutine is not null)
        StopCoroutine(m_pointerDownCoroutine);
      if (m_ShowProgress)
        m_ProgressIndicator.fillAmount = 0f;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(LongClickButton))]
    public class LongPressableButtonEditor : SelectableEditor
    {
      private GUIStyle m_BoldFoldoutStyle => new(EditorStyles.foldout) {
        fontStyle = FontStyle.Bold,
      };


      public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        // Main Switch
        var enableProperty = serializedObject.FindProperty(nameof(m_EnableLongClick));
        EditorGUILayout.PropertyField(enableProperty);
        EditorGUILayout.Space();

        var foldout1 = serializedObject.FindProperty(nameof(m_Foldout1));
        var foldout2 = serializedObject.FindProperty(nameof(m_Foldout2));

        if (enableProperty.boolValue) {
          if (foldout1.boolValue = EditorGUILayout.Foldout(foldout1.boolValue, "Long Click Properties", true, m_BoldFoldoutStyle)) {
            EditorGUI.indentLevel++;

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
          if (foldout2.boolValue = EditorGUILayout.Foldout(foldout2.boolValue, "Events", true, m_BoldFoldoutStyle)) {
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField(new GUIContent() { text = "Long Click Event", tooltip = "Invoked when the button is pressed for a long time and released." }, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(m_OnLongClick)));
            EditorGUILayout.LabelField(new GUIContent() { text = "Long Press Event", tooltip = "Invoked when the button is pressed long enough." }, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(m_OnLongPress)));
            EditorGUILayout.LabelField(new GUIContent() { text = "Short Click Event", tooltip = "Invoked when the button is pressed and released shortly." }, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(m_OnShortClick)));

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