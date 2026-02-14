using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SnakeController : MonoBehaviour
{
    public float forwardForce = 15f;     // Сила вперёд (газ)
    public float maxSpeed = 20f;         // Максимальная скорость (опционально)
    public float turnTorque = 100f;      // Сила поворота
    public float wiggleForce = 3f;       // Волнообразное ползание змеи
    public float wiggleFreq = 5f;        // Частота волны

    public Transform[] bodySegments;     // Массив сегментов тела (drag в инспекторе)

    private Rigidbody rb;

    // Мультипликаторы
    public float speedMult = 1.5f;       // От OffTrackDetector (on-track ускорение)
    public float boostMult = 1f;         // От бонусов (ускорение)
    public float slipTimeLeft = 0f;      // От шампуня (занос + замедление)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");     // W/S = газ/тормоз
        float horizontal = Input.GetAxis("Horizontal"); // A/D = поворот

        // Движение вперёд/назад
        if (Mathf.Abs(vertical) > 0.01f)
        {
            Vector3 force = transform.forward * forwardForce * vertical * speedMult * boostMult;
            rb.AddForce(force);

            // Лимит скорости
            if (rb.linearVelocity.magnitude > maxSpeed * speedMult * boostMult)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * (maxSpeed * speedMult * boostMult);
            }
        }

        // Поворот
        rb.AddTorque(Vector3.up * horizontal * turnTorque);

        // Волнообразное ползание (wiggle)
        float wiggle = Mathf.Sin(Time.time * wiggleFreq) * wiggleForce;
        rb.AddForce(transform.right * wiggle);

        // Slip от шампуня (занос + низкое сцепление)
        if (slipTimeLeft > 0)
        {
            slipTimeLeft -= Time.fixedDeltaTime;
            rb.linearDamping = 0.5f;  // Скользко
            rb.AddTorque(Vector3.up * Random.Range(-turnTorque * 3f, turnTorque * 3f));  // Random занос
            if (slipTimeLeft <= 0) rb.linearDamping = 2f;  // Вернуть нормальный drag
        }

        // Тело следует за головой (Lerp для плавности)
        FollowBody();
    }

    void FollowBody()
    {
        if (bodySegments.Length == 0) return;

        for (int i = 0; i < bodySegments.Length; i++)
        {
            Transform target = (i == 0) ? transform : bodySegments[i - 1];
            Vector3 targetPos = target.position - target.forward * 1f;

            bodySegments[i].position = Vector3.Lerp(bodySegments[i].position, targetPos, 20f * Time.deltaTime);
            bodySegments[i].rotation = Quaternion.Lerp(bodySegments[i].rotation, target.rotation, 20f * Time.deltaTime);
        }
    }

    // Методы для бонусов
    public void ApplyBoost(float mult, float duration)
    {
        boostMult = mult;
        CancelInvoke("ResetBoost");
        Invoke("ResetBoost", duration);
    }

    void ResetBoost()
    {
        boostMult = 1f;
    }

    public void ApplySlip(float duration)
    {
        slipTimeLeft = duration;
    }
}