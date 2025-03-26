using System.Collections;

using UnityEngine;
using UnityEngine.AI;

namespace TRIdle.Game.Controller.PlayerInternal
{
  using UI;
  using Skill;
  using Logics;
  using Logics.Attributes;
  using Logics.Extensions;

  /// <summary>
  /// This class is attatched to the player character gameobject.
  /// Controls automatic movements/motions of the character in a real world.
  /// Also provides coroutines' ownership for the actions of the player.
  /// </summary>
  public class PlayerMono : MonoBehaviour
  {
    private static PlayerMono s_Instance;
    public static PlayerMono GetInstance() {
      if (s_Instance == null)
        return new GameObject("PlayerMono").AddComponent<PlayerMono>();
      return s_Instance;
    }

    [SerializeField, ReadonlyField] protected Transform _Model;
    [SerializeField, ReadonlyField] protected Animator _Animator;
    [SerializeField, ReadonlyField] protected NavMeshAgent _NavMeshAgent;


    private Renderer _Renderer;

    #region Unity Callbacks
    private void Awake() {
      if (s_Instance != null) {
        Debug.LogError("PlayerMono is already instantiated.");
        Destroy(this);
        return;
      }
      s_Instance = this;
    }

    private void OnDrawGizmos() {
      if (FindRenderer()) // Also works in editor mode
        TDebug.DrawCube(_Renderer.bounds.center, _Renderer.bounds.extents, Color.cyan);
    }
    #endregion

    public void Setup(GameObject model/*TODO : Get serialized data from Player*/) {
      _Model = Instantiate(model, transform).transform;
      _Animator = _Model.GetComponentInChildren<Animator>();
      _NavMeshAgent = GetComponentInChildren<NavMeshAgent>();
      FindRenderer();
    }

    private bool FindRenderer() => _Renderer != null || transform.TryGetComponentInChildren(out _Renderer);

    #region Action Coroutine
    // TODO : Important changes
    // Now PlayrMono has a 3D world gameobject.
    // Edit below action features into a real world prototypes.

    public float delayDuration, delayElapsed;
    private ActionBase _DelayingAction;
    private Coroutine _DelayCoroutine;

    public void StartActionDelay(ActionBase action) {
      // same action : ignore
      if (action == _DelayingAction) return;
      // other action : stop current action (delay)
      if (_DelayCoroutine is not null) {
        StopCoroutine(_DelayCoroutine);
        if (_DelayingAction is not null) _DelayingAction.Progress = 0;
      }
      // null action : update and ignore
      if ((_DelayingAction = action) is null) return;
      // start delay
      this.Log("Start Delay!");
      _DelayCoroutine = StartCoroutine(DelayLoop());
    }

    IEnumerator DelayLoop() {
      while (_DelayingAction is not null) {
        delayElapsed = 0;
        delayDuration = _DelayingAction.Data.Duration.Value;

        while (delayElapsed < delayDuration) {
          delayElapsed += Time.DeltaTime;
          _DelayingAction.Progress = delayElapsed / delayDuration;
          yield return null;
        }

        _DelayingAction.Activate();
        _DelayingAction.Progress = delayElapsed = 0;
        UI_MainSceneController.Instance.Menu_Update();
      }
    }
    #endregion
  }
}