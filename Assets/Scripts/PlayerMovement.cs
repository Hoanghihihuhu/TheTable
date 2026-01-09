using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 movement;
    public float deadZone = 0.1f;

    [Header("Raycast Shooting")]
    public float fireRate = 0.15f;
    public float shootDistance = 20f;
    public LayerMask hitMask;

    public Transform firePoint;

    private float fireTimer;

    private Animator ani;
    public Camera cam;
    private Vector2 lookDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        // Lấy input 8 hướng (WASD / Arrow)
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        // Chuẩn hóa vector để đi chéo không nhanh hơn
        movement = movementInput.normalized;
        float speed = movement.magnitude;
        ani.SetFloat("Speed", speed);

        bool isMoving = speed > 0.01f;
        bool isHoldingAttack = Input.GetMouseButton(0);

        // ===== ATTACK STATES =====
        bool isAttackRun = isMoving && isHoldingAttack;
        bool isAttackIdle = !isMoving && !isHoldingAttack;

        ani.SetBool("IsAttackRun", isAttackRun);
        ani.SetBool("IsAttackIdle", isAttackIdle);
        CalculateAnimation();
        HandleRaycastShooting();
    }
    void FixedUpdate()
    {
        rb.velocity = movement * moveSpeed;
    }

    void CalculateAnimation()
    {
        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = mouseWorld - transform.position;
        lookDirection.Normalize();

        if (lookDirection.magnitude < deadZone)
        {
            return;
        }

        ani.SetFloat("LookX", lookDirection.x);
        ani.SetFloat("LookY", lookDirection.y);

        // ===== FIRE POINT ROTATION =====
        //float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        //firePoint.rotation = Quaternion.Euler(0, 0, angle);

        // Đẩy firepoint ra trước mặt
        firePoint.localPosition = lookDirection * 0.5f;
    }
    void HandleRaycastShooting()
    {
        if (!Input.GetMouseButton(0))
            return;

        fireTimer += Time.deltaTime;
        if (fireTimer < fireRate)
            return;

        fireTimer = 0f;

        RaycastHit2D hit = Physics2D.Raycast(
            firePoint.position,
            lookDirection,
            shootDistance,
            hitMask
        );

        Debug.DrawRay(
            firePoint.position,
            lookDirection * shootDistance,
            Color.red,
            0.1f
        );

        if (hit.collider != null)
        {
            // ===== HIT SOMETHING =====
            Debug.Log("Hit: " + hit.collider.name);

            // Damage (nếu có)
            // IDamageable dmg = hit.collider.GetComponent<IDamageable>();
            // if (dmg != null)
            // {
            //     dmg.TakeDamage(1);
            // }

            // Impact effect (nâng cấp ở dưới)
        }
    }
}

