using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SnakeController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speed = 30f;
    public float turnSpeed = 150f;

    [Header("Физика притяжения")]
    [Tooltip("Сила, прижимающая куб к земле. Попробуй 50-100 для эффекта тяжести.")]
    public float extraGravity = 70f;

    [Header("Ссылки и переменные")]
    public Transform visualModel; // Перетащи сюда дочерний куб-модель
    [HideInInspector] public float speedMult = 1f; // Для TrackSurfaceDetector
    [HideInInspector] public float boostMult = 1f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Масса не должна быть огромной, 1-5 вполне достаточно
        rb.mass = 5f;

        // Замораживаем физическое вращение, чтобы не было "вертолета"
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;

        // Включаем интерполяцию для плавности камеры
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        ApplyPhysics();
        HandleMovement(v);
        HandleRotation(h);

        if (visualModel != null)
        {
            AlignVisual();
        }
    }

    void ApplyPhysics()
    {
        // Дополнительная гравитация вниз (Acceleration игнорирует массу)
        rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
    }

    void HandleMovement(float input)
    {
        // Рассчитываем вектор движения вперед
        // Мы сохраняем текущий rb.linearVelocity.y, чтобы гравитация работала
        Vector3 moveDirection = transform.forward * input * speed * speedMult * boostMult;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
    }

    void HandleRotation(float input)
    {
        // Логический поворот родительского объекта
        if (Mathf.Abs(input) > 0.05f)
        {
            transform.Rotate(Vector3.up * input * turnSpeed * Time.fixedDeltaTime);
        }
    }

    void AlignVisual()
    {
        RaycastHit hit;
        // Пускаем луч вниз, чтобы определить наклон поверхности
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2.5f))
        {
            // Вычисляем нужный наклон на основе нормали поверхности
            Quaternion targetRot = Quaternion.FromToRotation(visualModel.up, hit.normal) * visualModel.rotation;
            // Плавно наклоняем только визуальную модельку
            visualModel.rotation = Quaternion.Slerp(visualModel.rotation, targetRot, Time.fixedDeltaTime * 10f);
        }
    }
}