using System.Collections;

using UnityEngine;

namespace TRIdle.Logics
{
  using Game;
  using Extensions;
  using Serialization;

  public class GameLoader : MonoBehaviour
  {
    public static GameLoader Instance { get; private set; }
    public GameLoader() { Instance = this; }

    private void Awake() => DontDestroyOnLoad(this);
    private void Start() => StartCoroutine(Init());

    private IEnumerator Init() {
      this.Log("Initializing game...");
      yield return Player.Instance.Load();
      yield return LocalizationLoader.Instance.Load();
      // Add more loaders here

      // Load the main scene
      this.Log("Game initialized. Loading main scene...");
      var loading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Main");
      while (!loading.isDone) yield return null; // Wait until the scene is loaded
    }
  }
}