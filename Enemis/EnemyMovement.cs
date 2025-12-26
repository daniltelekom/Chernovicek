using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    Transform player;
    Rigidbody rb;
    EnemyHealth hp;

    void Awake()
    {
        player = FindObjectOfType<PlayerStats>()?.transform;
        rb = GetComponent<Rigidbody>();
        hp = GetComponent<EnemyHealth>();
    }

    void FixedUpdate()
    {
        if (player == null) return;
        if (hp != null && hp.IsDead) return;

        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.1f) return;

        Vector3 move = dir.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }
}
