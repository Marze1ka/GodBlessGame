using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleMobSpawner : MonoBehaviour
{
    [Header("Кого спавним?")]
    public GameObject mobPrefab;

    [Header("Настройки")]
    public bool spawnOnStart = true;

    private GameObject _spawnedMob;

    private void Start()
    {
        if (spawnOnStart)
        {
            Spawn();
        }
    }

    public string GetSaveId()
    {
        Vector3 pos = transform.position;
        return $"{SceneManager.GetActiveScene().name}:Spawner:{gameObject.name}:{pos.x:F2}:{pos.y:F2}:{pos.z:F2}";
    }

    public bool MatchesSaveId(string saveId)
    {
        return GetSaveId() == saveId;
    }

    public GameObject Spawn()
    {
        if (mobPrefab == null)
        {
            Debug.LogWarning("Спавнер на объекте " + gameObject.name + " пустой! Забыли префаб.");
            return null;
        }

        if (_spawnedMob != null)
        {
            return _spawnedMob;
        }

        _spawnedMob = Instantiate(mobPrefab, transform.position, transform.rotation);

        EnemySaveIdentity identity = _spawnedMob.GetComponent<EnemySaveIdentity>();
        if (identity == null)
        {
            identity = _spawnedMob.AddComponent<EnemySaveIdentity>();
        }

        identity.Configure(GetSaveId());
        return _spawnedMob;
    }

    public void ClearTrackedMob(GameObject mob)
    {
        if (_spawnedMob == mob)
        {
            _spawnedMob = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
