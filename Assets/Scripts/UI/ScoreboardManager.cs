using UnityEngine;
using TMPro; // Обязательно для текста

public class ScoreboardManager : MonoBehaviour
{
    // Одиночка (Singleton) — чтобы из любого скрипта написать ScoreboardManager.Instance
    public static ScoreboardManager Instance;

    [Header("Настройки интерфейса")]
    public TextMeshProUGUI scoreText; // Ссылка на текст на экране

    [Header("Событие: Босс (3 убийства)")]
    public GameObject bossPrefab;    // Префаб босса
    public Transform bossSpawnPoint; // Точка появления босса

    [Header("Событие: Победа (5 убийств)")]
    public AudioSource victoryAudio; // Объект со звуком победы

    private int killCount = 0;

    private void Awake()
    {
        // Инициализация одиночки
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    // Эту функцию мы будем вызывать из скрипта Health при смерти моба
    public void AddKill()
    {
        killCount++;
        UpdateUI();
        Debug.Log("Счетчик: " + killCount);

        // Проверка условий ТЗ
        if (killCount == 3)
        {
            SpawnBoss();
        }

        if (killCount == 5)
        {
            PlayVictoryMusic();
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "ПОБЕЖДЕНО: " + killCount;
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
            Debug.Log("<color=red>ВНИМАНИЕ: БОСС ПОЯВИЛСЯ!</color>");
        }
    }

    private void PlayVictoryMusic()
    {
        if (victoryAudio != null)
        {
            victoryAudio.Play();
            Debug.Log("<color=green>ПОБЕДА! ИГРАЕТ МУЗЫКА!</color>");
        }
    }
}