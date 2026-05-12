using UnityEngine;

public class SaveService : ISaveService
{
    public void SaveGame()
    {
        // Ищем объект именно с тегом Player
        GameObject playerObj = GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            Health playerHealth = playerObj.GetComponent<Health>();
            PlayerPrefs.SetFloat("PlayerHP", playerHealth.currentHealth);
            PlayerPrefs.Save(); // Принудительно записываем на диск
            Debug.Log("<color=green>ИГРА СОХРАНЕНА!</color> Записано HP: " + playerHealth.currentHealth);
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
            Health playerHealth = playerObj.GetComponent<Health>();
            float savedHP = PlayerPrefs.GetFloat("PlayerHP");

            playerHealth.currentHealth = savedHP;

            // ОБЯЗАТЕЛЬНО обновляем полоску здоровья визуально
            if (playerHealth.healthSlider != null)
                playerHealth.healthSlider.value = savedHP;

            Debug.Log("<color=cyan>ЗАГРУЗКА ЗАВЕРШЕНА!</color> Установлено HP: " + savedHP);
        }
        else
        {
            Debug.LogWarning("Сохранение не найдено или игрок отсутствует.");
        }
    }
}