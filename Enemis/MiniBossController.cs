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

    public GameObject slamVfxPrefab;
    public float slamDelay = 0.6f;

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
    if (slamVfxPrefab != null)
    {
        var vfx = Instantiate(
            slamVfxPrefab,
            new Vector3(transform.position.x, 0.01f, transform.position.z),
            Quaternion.identity
        );

        var ctrl = vfx.GetComponent<SlamVFX>();
        if (ctrl != null)
            ctrl.SetRadius(slamRadius);
    }

    Invoke(nameof(DoSlamDamage), slamDelay);
}

void DoSlamDamage()
{
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

    Debug.Log("MiniBoss Slam HIT");
}

    void Dash()
{
    if (rb == null || player == null) return;

    StartCoroutine(DashRoutine());
}

System.Collections.IEnumerator DashRoutine()
{
    // телеграф — лёгкое сжатие
    Vector3 originalScale = transform.localScale;
    transform.localScale = originalScale * 0.9f;

    yield return new WaitForSeconds(0.4f);

    transform.localScale = originalScale;

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
