using UnityEngine;

public class LevelBootstrapper : MonoBehaviour
{
    public PlayerView playerView;
    public PlayerController playerController;

    private void Start() // Используем Start, чтобы все объекты успели появиться
    {
        GameBootstrapper.EnsureInitialized();

        if (playerView == null || playerController == null)
        {
            Debug.LogError("LEVEL BOOTSTRAPPER: Ссылки на View или Controller не установлены в инспекторе!");
            return;
        }

        // 1. Создаем Модель (данные)
        PlayerModel playerModel = new PlayerModel();

        // 2. Запускаем Контроллер
        playerController.Initialize(playerModel, playerView);

        Debug.Log("<color=yellow>LEVEL BOOTSTRAPPER: Игрок успешно инициализирован!</color>");

        // Скрываем курсор при старте
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
