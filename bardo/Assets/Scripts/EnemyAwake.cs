using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemySleepAI2D : MonoBehaviour
{
    public enum State { Dormant, Chase }

    [Header("Refs")]
    [SerializeField] Transform target;
    NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Detecção")]
    [SerializeField] float wakeRadius = 8f;
    [SerializeField] float sleepRadius = 12f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] bool requireLineOfSight = false;
    [SerializeField] LayerMask losObstacles2D;

    [Header("Movimento e animação")]
    private Vector3 lastPosition;
    private Vector3 initialScale;

    State state = State.Dormant;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (target == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) target = go.transform;
        }

        initialScale = transform.localScale;
        agent.isStopped = true; // começa dormindo
    }

    void OnEnable()
    {
        if (!agent.isOnNavMesh && NavMesh.SamplePosition(transform.position, out var hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.Warp(hit.position);
        }

        lastPosition = transform.position;
        InvokeRepeating(nameof(SenseLoop), 0f, 0.2f);
    }

    void OnDisable() => CancelInvoke(nameof(SenseLoop));

    void Update()
    {
        // Detecta se está se movendo (para animar)
        float velocity = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;
        bool isMoving = velocity > 0.01f && state == State.Chase;

        // Atualiza animações
        animator.SetBool("isRunning", isMoving);

        // Flip do sprite (igual ao Player)
        Vector3 dir = transform.position - lastPosition;
        if (dir.x < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else if (dir.x > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);

        lastPosition = transform.position;
    }

    void SenseLoop()
    {
        if (!target) return;

        Vector2 me = new Vector2(transform.position.x, transform.position.z);
        Vector2 tp = new Vector2(target.position.x, target.position.z);
        float dist = Vector2.Distance(me, tp);

        bool playerOnLayer = Physics2D.OverlapCircle(
            new Vector2(target.position.x, target.position.y),
            0.1f,
            playerLayer
        );

        bool inWake = dist <= wakeRadius && playerOnLayer;
        bool outSleep = dist >= sleepRadius || !playerOnLayer;

        if (requireLineOfSight && inWake)
        {
            Vector2 a = new Vector2(transform.position.x, transform.position.y);
            Vector2 b = new Vector2(target.position.x, target.position.y);
            var hit2D = Physics2D.Linecast(a, b, losObstacles2D);
            if (hit2D.collider != null) inWake = false;
        }

        switch (state)
        {
            case State.Dormant:
                if (inWake) StartChase();
                break;

            case State.Chase:
                if (outSleep) GoDormant();
                else if (agent.enabled && agent.isOnNavMesh)
                    agent.SetDestination(target.position);
                break;
        }
    }

    void StartChase()
    {
        state = State.Chase;
        agent.isStopped = false;
        if (agent.enabled && agent.isOnNavMesh && target)
            agent.SetDestination(target.position);
        animator.SetBool("isRunning", true);
    }

    void GoDormant()
    {
        state = State.Dormant;
        agent.isStopped = true;
        animator.SetBool("isRunning", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green; Gizmos.DrawWireSphere(transform.position, wakeRadius);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, sleepRadius);
    }
}
