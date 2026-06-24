using System;
using System.Collections;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public static GameBootstrapper Instance { get; private set; }
    public static SaveLoadInteractor SaveInteractor { get; private set; }
    public static IAudioService AudioService { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        EnsureInitialized();
    }

    public static void EnsureInitialized()
    {
        if (Instance != null && SaveInteractor != null && AudioService != null)
        {
            return;
        }

        GameBootstrapper bootstrapper = FindAnyObjectByType<GameBootstrapper>();
        if (bootstrapper == null)
        {
            GameObject bootstrapperObject = new GameObject(nameof(GameBootstrapper));
            bootstrapper = bootstrapperObject.AddComponent<GameBootstrapper>();
        }

        bootstrapper.InitializeServices();
    }

    private void Awake()
    {
        GameBootstrapper[] bootstrappers = UnityEngine.Object.FindObjectsByType<GameBootstrapper>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (bootstrappers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeServices();
    }

    private void InitializeServices()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (AudioService == null)
        {
            AudioService = new AudioService();
        }

        if (SaveInteractor == null)
        {
            var playerRepo = new LocalPlayerDataRepository();
            var enemyRepo = new LocalEnemyRepository();
            SaveInteractor = new SaveLoadInteractor(playerRepo, enemyRepo);
        }

        Debug.Log("GameBootstrapper: systems initialized.");
    }

    public void RunNextFrame(Action action)
    {
        StartCoroutine(RunNextFrameRoutine(action));
    }

    private IEnumerator RunNextFrameRoutine(Action action)
    {
        yield return null;
        action?.Invoke();
    }
}
