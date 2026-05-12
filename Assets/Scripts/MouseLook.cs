using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Настройки чувствительности")]
    public float mouseSensitivity = 100f; // Скорость вращения

    [Header("Ссылки")]
    public Transform playerBody; // Сюда перетащи объект Player в Инспекторе

    private float xRotation = 0f;

    void Start()
    {
        // При старте игры блокируем курсор в центре экрана и делаем его невидимым
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Если игра на паузе (Time.timeScale == 0), камеру не вращаем
        if (Time.timeScale == 0f) return;

        // 1. Получаем ввод от мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Рассчитываем вертикальный поворот (вверх-вниз)
        xRotation -= mouseY;
        // Ограничиваем поворот, чтобы нельзя было "сделать сальто" головой
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Применяем вращение к самой камере
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 3. Вращаем всё тело игрока по горизонтали (влево-вправо)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}