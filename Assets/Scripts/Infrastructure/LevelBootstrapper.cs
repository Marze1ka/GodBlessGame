using UnityEngine;

public class LevelBootstrapper : MonoBehaviour
{
    private void Start()
    {
        // Здесь можно автоматически настроить камеру или спавн игрока
        Debug.Log("Сцена игры: Объекты настроены.");

        // Скрываем мышку при старте уровня
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}