using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // ѕоворачиваем полоску так же, как повернута камера
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}