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
    [SerializeField] protected UnityEvent _OnLongClick, _OnLongPress, _OnShortClick, _OnAnyPress;

    [SerializeField, Tooltip("Whether to show the progress indicator while long pressing.")]
    private bool _ShowProgress;
    [SerializeField] private Image _ProgressIndicator;

    [SerializeField, Tooltip("After this time(seconds) the press is considered as long pressing.")]
    private float _LongClickThreshold = 0.5f;
    [SerializeField, Tooltip("After this time(seconds) OnLongPress event will be invoked.")]
    private float _LongPressDuration = 1f;

    [SerializeField] private bool _EnableLongClick = true;
    private enum PressState { Free, Press, LongPress, LongEnoughPress }
    [SerializeField, ReadonlyField] private PressState _State;
#if UNITY_EDITOR
    [SerializeField, HideInInspector] private bool _Initialized = false, m_Foldout1, m_Foldout2;
#endif
    #endregion

#if UNITY_EDITOR
    protected override void OnValidate() {
      base.OnValidate();
      if (_Initialized is false && _ShowProgress && Application.isPlaying is false) {
        // Create default progress indicator if it's not set.
        if (_ProgressIndicator == null) {
          var go = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/Long Click Button - Progress Indicator.prefab");
          _ProgressIndicator = Instantiate(go, transform).GetComponent<Image>();
          this.Log("Default progress indicator created. Please check it in inspector");
        }
        _Initialized = true;
      }
    }
#endif

    private bool IndicatorEnabled => _ShowProgress && _ProgressIndicator != null;
    private float _PressedTime = float.MaxValue;
    private Coroutine _PointerDownCoroutine;
    public override void OnPointerClick(PointerEventData eventData) {
      if (_EnableLongClick) {
        if (_State is PressState.Press)
          _OnShortClick?.Invoke();
        else if (_State > PressState.Press)
          _OnLongClick?.Invoke();
      }

      base.OnPointerClick(eventData);
      _State = PressState.Free;
    }
    public override void OnPointerDown(PointerEventData eventData) {
      // Process default behaviour
      base.OnPointerDown(eventData);
      if (_EnableLongClick)
        _OnAnyPress?.Invoke();

      if (_PointerDownCoroutine is not null)
        TryAbortLongPress();

      _PressedTime = Time.unscaledTime;
      if (IndicatorEnabled) {
        _ProgressIndicator.gameObject.SetActive(true);
        _ProgressIndicator.rectTransform.position = eventData.position;
      }
      _PointerDownCoroutine = StartCoroutine(PressDownEnumerator());
    }
    public override void OnPointerUp(PointerEventData eventData) {
      base.OnPointerUp(eventData);

      if (_EnableLongClick)
        TryAbortLongPress();
    }
    public override void OnPointerExit(PointerEventData eventData) {
      base.OnPointerExit(eventData);

      if (_EnableLongClick)
        TryAbortLongPress();
    }

    private IEnumerator PressDownEnumerator() {
      _State = PressState.Press;
      _PressedTime = 0;
      while (_PressedTime < _LongPressDuration) {
        if (IndicatorEnabled) {
          var color = _ProgressIndicator.color;
          var lerp = Mathf.Clamp01((_PressedTime - _LongClickThreshold) / (_LongPressDuration - _LongClickThreshold));
          color.a = lerp;
          _ProgressIndicator.color = color;
          _ProgressIndicator.fillAmount = _PressedTime / _LongPressDuration;
        }
        if (_State is PressState.Press && _PressedTime >= _LongClickThreshold)
          _State = PressState.LongPress;
        yield return null;
        _PressedTime += Time.deltaTime;
      }
      _OnLongPress?.Invoke();
      ResetIndicator();
      _State = PressState.LongEnoughPress;
    }
    private void TryAbortLongPress() {
      if (_PointerDownCoroutine is not null) {
        StopCoroutine(_PointerDownCoroutine);
        _PointerDownCoroutine = null;
      }
      if (IndicatorEnabled)
        ResetIndicator();
    }
    private void ResetIndicator() {
      _ProgressIndicator.fillAmount = 0f;
      _ProgressIndicator.gameObject.SetActive(false);
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
        var enableProperty = serializedObject.FindProperty(nameof(_EnableLongClick));
        EditorGUILayout.PropertyField(enableProperty);
        var threshold = serializedObject.FindProperty(nameof(_LongClickThreshold));
        EditorGUILayout.Space();

        var foldout1 = serializedObject.FindProperty(nameof(m_Foldout1));
        var foldout2 = serializedObject.FindProperty(nameof(m_Foldout2));

        if (enableProperty.boolValue) {
          EditorGUI.indentLevel++;

          var duration = serializedObject.FindProperty(nameof(_LongPressDuration));
          threshold.floatValue = EditorGUILayout.Slider(threshold.displayName, threshold.floatValue, .01f, duration.floatValue);
          duration.floatValue = EditorGUILayout.Slider(duration.displayName, duration.floatValue, threshold.floatValue, 5);

          EditorGUI.indentLevel--;

          if (foldout1.boolValue = EditorGUILayout.Foldout(foldout1.boolValue, "Long Click Properties", true, BoldFoldoutStyle)) {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_State)));
            var showProgressProperty = serializedObject.FindProperty(nameof(_ShowProgress));
            EditorGUILayout.PropertyField(showProgressProperty);
            if (showProgressProperty.boolValue) {
              var progressIndicatorProperty = serializedObject.FindProperty(nameof(_ProgressIndicator));
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_OnLongClick)));
            EditorGUILayout.LabelField(new GUIContent() { text = "Long Press Event", tooltip = "Invoked when the button is pressed long enough." }, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_OnLongPress)));
            EditorGUILayout.LabelField(new GUIContent() { text = "Short Click Event", tooltip = "Invoked when the button is pressed and released shortly." }, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_OnShortClick)));
            EditorGUILayout.LabelField(new GUIContent() { text = "Press Event", tooltip = "Invoked instantly when the button is pressed." }, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_OnAnyPress)));

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