using UnityEngine;

public class SaveService : ISaveService
{
    public void SaveGame()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("Ошибка сохранения: На объекте Player нет PlayerController!");
                return;
            }

            float currentHealth = playerController.GetModel().Health;
            PlayerPrefs.SetFloat("PlayerHP", currentHealth);
            PlayerPrefs.Save();
            Debug.Log("<color=green>ИГРА СОХРАНЕНА!</color> Записано HP: " + currentHealth);
        }
        else
        {
            Debug.LogError("Ошибка сохранения: Объект с тегом 'Player' не найден!");
        }
    }

    public void LoadGame()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null && PlayerPrefs.HasKey("PlayerHP"))
        {
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("Ошибка загрузки: На объекте Player нет PlayerController!");
                return;
            }

            float savedHP = PlayerPrefs.GetFloat("PlayerHP");
            playerController.SetHealthFromSave(savedHP);

            Debug.Log("<color=cyan>ЗАГРУЗКА ЗАВЕРШЕНА!</color> Установлено HP: " + savedHP);
        }
        else
        {
            Debug.LogWarning("Сохранение не найдено или игрок отсутствует.");
        }
    }
}
