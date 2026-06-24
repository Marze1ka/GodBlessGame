using UnityEngine;

public class EnemySaveIdentity : MonoBehaviour
{
    [SerializeField] private string saveId;

    public string SaveId => saveId;

    public void Configure(string newSaveId)
    {
        saveId = newSaveId;
    }
}
