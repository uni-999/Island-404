using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 10f, -15f);
    public float smoothPosition = 0.125f;
    public float smoothRotation = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // Позиция: плавно движемся за целью с учетом её поворота
        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothPosition);

        // Взгляд: смотрим на точку впереди змеи, но игнорируем наклон Y
        Vector3 lookPoint = target.position + target.forward * 5f;
        Vector3 direction = lookPoint - transform.position;
        direction.y = 0; // БЛОКИРОВКА ВЕРТИКАЛЬНОГО НАКЛОНА

        if (direction != Vector3.zero)
        {
            Quaternion desiredRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, smoothRotation * Time.deltaTime);
        }
    }
}