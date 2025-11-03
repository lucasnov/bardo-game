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
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}