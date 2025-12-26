using UnityEngine;

public class MiniBossController : MonoBehaviour
{
    [Header("Attack")]
    public float slamCooldown = 6f;
    public float slamRadius = 3.5f;
    public int slamDamage = 18;

    [Header("Dash")]
    public float dashCooldown = 10f;
    public float dashForce = 16f;

    float slamTimer;
    float dashTimer;

    Transform player;
    EnemyHealth hp;
    Rigidbody rb;

    void Awake()
    {
        player = FindObjectOfType<PlayerStats>()?.transform;
        hp = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody>();

        slamTimer = Random.Range(1f, slamCooldown);
        dashTimer = Random.Range(2f, dashCooldown);
    }

    void Update()
    {
        if (player == null || hp == null || hp.IsDead) return;

        slamTimer -= Time.deltaTime;
        dashTimer -= Time.deltaTime;

        if (slamTimer <= 0f)
        {
            Slam();
            slamTimer = slamCooldown;
        }

        if (dashTimer <= 0f)
        {
            Dash();
            dashTimer = dashCooldown;
        }
    }

    void Slam()
    {
        // визуал потом, сейчас логика
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            slamRadius,
            LayerMask.GetMask("Player")
        );

        foreach (var h in hits)
        {
            var p = h.GetComponent<PlayerStats>();
            if (p != null)
                p.TakeDamage(slamDamage);
        }

        Debug.Log("MiniBoss Slam");
    }

    void Dash()
    {
        if (rb == null) return;

        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;

        rb.AddForce(dir.normalized * dashForce, ForceMode.Impulse);
        Debug.Log("MiniBoss Dash");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, slamRadius);
    }
}
