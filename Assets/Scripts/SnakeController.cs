using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 1.8f;
    public Transform[] bodySegments; // Сюда перетащим сегменты в инспекторе

    void Update()
    {
        // Поворот стрелками или A/D
        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, horizontal * turnSpeed, 0);

        // Движение вперёд
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Следование сегментов
        FollowBody();
    }

    void FollowBody()
    {
        if (bodySegments.Length == 0) return;

        // Первый сегмент следует за головой
        bodySegments[0].position = Vector3.Lerp(
            bodySegments[0].position,
            transform.position - transform.forward * 1f,
            0.3f
        );
        bodySegments[0].forward = transform.forward;

        // Остальные следуют за предыдущими
        for (int i = 1; i < bodySegments.Length; i++)
        {
            bodySegments[i].position = Vector3.Lerp(
                bodySegments[i].position,
                bodySegments[i - 1].position - bodySegments[i - 1].forward * 1f,
                0.3f
            );
            bodySegments[i].forward = bodySegments[i - 1].forward;
        }
    }
}