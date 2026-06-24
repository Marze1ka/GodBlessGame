using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadInteractor
{
    private readonly IPlayerDataRepository _playerRepo;
    private readonly IEnemyRepository _enemyRepo;

    private PlayerData _lastSavedPlayerData;
    private EnemySaveData _lastSavedEnemyData;
    private PlayerData _pendingPlayerData;
    private EnemySaveData _pendingEnemyData;

    public SaveLoadInteractor(IPlayerDataRepository playerRepo, IEnemyRepository enemyRepo)
    {
        _playerRepo = playerRepo;
        _enemyRepo = enemyRepo;
    }

    public void SaveGame()
    {
        GameObject playerObj = FindPlayerObject();
        if (playerObj != null)
        {
            PlayerController pc = playerObj.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.EnsureInitialized();
                PlayerData pData = new PlayerData
                {
                    HP = pc.GetModel().Health,
                    Position = playerObj.transform.position,
                    SceneName = SceneManager.GetActiveScene().name
                };
                _lastSavedPlayerData = pData;
                _playerRepo.Save(pData);
            }
        }

        EnemySaveData eSaveData = new EnemySaveData
        {
            sceneName = SceneManager.GetActiveScene().name
        };

        if (ScoreboardManager.Instance != null)
        {
            eSaveData.killCount = ScoreboardManager.Instance.KillCount;
            eSaveData.bossPresent = ScoreboardManager.Instance.HasBossAlive;
        }

        Health[] allUnits = Object.FindObjectsByType<Health>(FindObjectsSortMode.None);
        foreach (var unit in allUnits)
        {
            if (unit.playerControlled || unit.currentHealth <= 0f)
            {
                continue;
            }

            EnemySaveIdentity identity = unit.GetComponent<EnemySaveIdentity>();
            if (identity == null || string.IsNullOrEmpty(identity.SaveId))
            {
                continue;
            }

            eSaveData.allEnemies.Add(new EnemyData
            {
                enemyID = unit.gameObject.name,
                saveId = identity.SaveId,
                currentHP = unit.currentHealth,
                position = unit.transform.position
            });
        }

        _lastSavedEnemyData = eSaveData;
        _enemyRepo.Save(eSaveData);
        Debug.Log("SaveLoadInteractor: save completed.");
    }

    public void LoadGame()
    {
        PlayerData pData = _lastSavedPlayerData ?? _playerRepo.Load();
        EnemySaveData eData = _lastSavedEnemyData ?? _enemyRepo.Load();

        if (pData == null || string.IsNullOrEmpty(pData.SceneName))
        {
            Debug.LogWarning("SaveLoadInteractor: player save not found.");
            return;
        }

        _pendingPlayerData = pData;
        _pendingEnemyData = eData;

        if (SceneManager.GetActiveScene().name == pData.SceneName)
        {
            ApplyPendingSave();
            return;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(pData.SceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_pendingPlayerData == null || scene.name != _pendingPlayerData.SceneName)
        {
            return;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (GameBootstrapper.Instance != null)
        {
            GameBootstrapper.Instance.RunNextFrame(ApplyPendingSave);
        }
        else
        {
            ApplyPendingSave();
        }
    }

    private void ApplyPendingSave()
    {
        if (_pendingPlayerData == null)
        {
            return;
        }

        RestorePlayer(_pendingPlayerData);
        RestoreWorld(_pendingEnemyData);

        _pendingPlayerData = null;
        _pendingEnemyData = null;
    }

    private void RestorePlayer(PlayerData pData)
    {
        GameObject playerObj = FindPlayerObject();
        if (playerObj == null)
        {
            if (GameBootstrapper.Instance != null)
            {
                GameBootstrapper.Instance.RunNextFrame(() => RestorePlayer(pData));
            }
            return;
        }

        PlayerController pc = playerObj.GetComponent<PlayerController>();
        CharacterController cc = playerObj.GetComponent<CharacterController>();

        if (pc != null)
        {
            pc.EnsureInitialized();
        }

        if (cc != null) cc.enabled = false;
        playerObj.transform.position = pData.Position;
        if (pc != null) pc.SetHealthFromSave(pData.HP);
        if (cc != null) cc.enabled = true;
        Physics.SyncTransforms();
    }

    private GameObject FindPlayerObject()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            return playerObj;
        }

        PlayerController playerController = Object.FindAnyObjectByType<PlayerController>();
        return playerController != null ? playerController.gameObject : null;
    }

    private void RestoreWorld(EnemySaveData eData)
    {
        if (eData == null)
        {
            return;
        }

        if (ScoreboardManager.Instance != null)
        {
            ScoreboardManager.Instance.RestoreState(eData.killCount, eData.bossPresent);
        }

        Dictionary<string, EnemyData> savedEnemies = new Dictionary<string, EnemyData>();
        foreach (EnemyData enemyData in eData.allEnemies)
        {
            if (!string.IsNullOrEmpty(enemyData.saveId))
            {
                savedEnemies[enemyData.saveId] = enemyData;
            }
        }

        foreach (Health health in Object.FindObjectsByType<Health>(FindObjectsSortMode.None))
        {
            if (health.playerControlled)
            {
                continue;
            }

            EnemySaveIdentity identity = health.GetComponent<EnemySaveIdentity>();
            if (identity == null || string.IsNullOrEmpty(identity.SaveId))
            {
                continue;
            }

            if (!savedEnemies.ContainsKey(identity.SaveId))
            {
                DestroyEnemyObject(health.gameObject);
            }
        }

        foreach (EnemyData enemyData in savedEnemies.Values)
        {
            Health enemyHealth = FindOrSpawnEnemy(enemyData.saveId);
            if (enemyHealth == null)
            {
                continue;
            }

            UnityEngine.AI.NavMeshAgent agent = enemyHealth.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) agent.enabled = false;

            enemyHealth.transform.position = enemyData.position;
            enemyHealth.currentHealth = enemyData.currentHP;
            if (enemyHealth.healthSlider != null)
            {
                enemyHealth.healthSlider.value = enemyData.currentHP;
            }

            if (agent != null) agent.enabled = true;
        }
    }

    private Health FindOrSpawnEnemy(string saveId)
    {
        foreach (EnemySaveIdentity identity in Object.FindObjectsByType<EnemySaveIdentity>(FindObjectsSortMode.None))
        {
            if (identity.SaveId == saveId)
            {
                return identity.GetComponent<Health>();
            }
        }

        if (saveId.EndsWith(":Boss"))
        {
            if (ScoreboardManager.Instance != null)
            {
                GameObject boss = ScoreboardManager.Instance.SpawnBossIfNeeded();
                if (boss != null)
                {
                    return boss.GetComponent<Health>();
                }
            }

            return null;
        }

        foreach (SimpleMobSpawner spawner in Object.FindObjectsByType<SimpleMobSpawner>(FindObjectsSortMode.None))
        {
            if (!spawner.MatchesSaveId(saveId))
            {
                continue;
            }

            GameObject spawned = spawner.Spawn();
            return spawned != null ? spawned.GetComponent<Health>() : null;
        }

        return null;
    }

    private void DestroyEnemyObject(GameObject enemyObject)
    {
        EnemySaveIdentity identity = enemyObject.GetComponent<EnemySaveIdentity>();
        if (identity != null && !identity.SaveId.EndsWith(":Boss"))
        {
            foreach (SimpleMobSpawner spawner in Object.FindObjectsByType<SimpleMobSpawner>(FindObjectsSortMode.None))
            {
                if (spawner.MatchesSaveId(identity.SaveId))
                {
                    spawner.ClearTrackedMob(enemyObject);
                    break;
                }
            }
        }

        Object.Destroy(enemyObject);
    }
}
