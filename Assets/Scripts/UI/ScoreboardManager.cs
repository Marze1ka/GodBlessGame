using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreboardManager : MonoBehaviour
{
    public static ScoreboardManager Instance;

    [Header("Настройки интерфейса")]
    public TextMeshProUGUI scoreText;

    [Header("Событие: Босс (3 убийства)")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    [Header("Событие: Победа (5 убийств)")]
    public AudioSource victoryAudio;

    private int killCount;
    private GameObject spawnedBoss;

    public int KillCount => killCount;
    public bool HasBossAlive =>
        spawnedBoss != null &&
        spawnedBoss.GetComponent<Health>() != null &&
        spawnedBoss.GetComponent<Health>().currentHealth > 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddKill()
    {
        killCount++;
        UpdateUI();
        Debug.Log("Счетчик: " + killCount);

        if (killCount == 3)
        {
            SpawnBossIfNeeded();
        }

        if (killCount == 5)
        {
            PlayVictoryMusic();
        }
    }

    public GameObject SpawnBossIfNeeded()
    {
        if (spawnedBoss != null)
        {
            return spawnedBoss;
        }

        if (bossPrefab == null || bossSpawnPoint == null)
        {
            return null;
        }

        spawnedBoss = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);

        EnemySaveIdentity identity = spawnedBoss.GetComponent<EnemySaveIdentity>();
        if (identity == null)
        {
            identity = spawnedBoss.AddComponent<EnemySaveIdentity>();
        }

        identity.Configure($"{SceneManager.GetActiveScene().name}:Boss");
        Debug.Log("<color=red>ВНИМАНИЕ: БОСС ПОЯВИЛСЯ!</color>");
        return spawnedBoss;
    }

    public void RestoreState(int restoredKills, bool restoreBoss)
    {
        killCount = restoredKills;
        UpdateUI();

        if (restoreBoss)
        {
            SpawnBossIfNeeded();
        }
        else if (spawnedBoss != null)
        {
            Destroy(spawnedBoss);
            spawnedBoss = null;
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "ПОБЕЖДЕНО: " + killCount;
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
