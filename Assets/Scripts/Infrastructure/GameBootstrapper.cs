using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public static SaveLoadInteractor SaveInteractor { get; private set; }
    public static IAudioService AudioService { get; private set; }

    private void Awake()
    {
        // Проверка на дубликаты Bootstrapper
        GameBootstrapper[] bootstrappers = Object.FindObjectsByType<GameBootstrapper>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (bootstrappers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Инициализация сервисов и репозиториев
        AudioService = new AudioService();

        var playerRepo = new LocalPlayerDataRepository();
        var enemyRepo = new LocalEnemyRepository();

        // Теперь передаем ДВА аргумента, и интерактор их примет
        SaveInteractor = new SaveLoadInteractor(playerRepo, enemyRepo);

        Debug.Log("GameBootstrapper: Все системы и репозитории запущены.");
    }
}