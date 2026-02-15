using UnityEngine;

public class SnakeBotAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 12f;
    public float rotationSpeed = 5f;
    public float waypointThreshold = 3f;

    private int currentWaypointIndex = 0;
    private Rigidbody rb;

    void Start() { rb = GetComponent<Rigidbody>(); }

    void FixedUpdate()
    {
        if (RaceManager.Instance == null || !RaceManager.Instance.isRaceStarted) return;
        if (waypoints == null || waypoints.Length == 0) return;

        Vector3 targetPos = waypoints[currentWaypointIndex].position;
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        }

        rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, targetPos) < waypointThreshold)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length) currentWaypointIndex = waypoints.Length - 1;
        }
    }
}