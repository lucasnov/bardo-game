using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class playerMovement : MonoBehaviour
{
    private float andaLateral;
    private float andaVertical;
    [SerializeField] private Animator animator;
    public float speed = 5f;
    Rigidbody2D rb;
    Vector2 input;
    private Vector3 initialScale;

    // Projectile
    [Header("Projectile")]
    public ProjectileBehaviour projectilePrefab;
    public Transform LaunchOffset;
    public AudioSource projectSound;

    [Header("Projectile Cooldown")]
    public float shootCooldown = 0.3f;   // tempo entre tiros (pode ajustar no Inspector)
    private float nextShootTime = 0f;    // controle interno

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Para 2D Top-down ou Side sem pulo
        initialScale = transform.localScale;
    }

    void Update()
    {
        andaLateral = Input.GetAxisRaw("Horizontal");
        andaVertical = Input.GetAxisRaw("Vertical");
        input = new Vector2(andaLateral, andaVertical).normalized;

        // Flip do GameObject conforme direcao horizontal
        if (andaLateral < 0f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }
        else if (andaLateral > 0f)
        {
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }

        // Add animations
        if (andaLateral != 0 || andaVertical != 0)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        // Skill area Q
        if (Input.GetKeyDown(KeyCode.Q)) // ou novo Input System
        {
            GetComponent<AreaSkill>()?.Cast();
        }

        // Launch projectile with Space + cooldown
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextShootTime)
        {
            nextShootTime = Time.time + shootCooldown;

            if (projectSound != null)
                projectSound.Play();

            // Launch projectile from LaunchOffset position
            var projectile = Instantiate(projectilePrefab, LaunchOffset.position, Quaternion.identity);

            // Set projectile direction based on player facing direction
            if (transform.localScale.x < 0)
            {
                projectile.transform.rotation = Quaternion.Euler(0, 0, 180); // Facing left
            }
            else
            {
                projectile.transform.rotation = Quaternion.Euler(0, 0, 0); // Facing right
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}
