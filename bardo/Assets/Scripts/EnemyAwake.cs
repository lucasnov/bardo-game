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
    }

    void OnEnable()
    {
        if (!agent.isOnNavMesh && NavMesh.SamplePosition(transform.position, out var hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            agent.Warp(hit.position);
        }

        SetAgentStopped(true);

        lastPosition = transform.position;
        InvokeRepeating(nameof(SenseLoop), 0f, 0.2f);
    }

    void OnDisable() => CancelInvoke(nameof(SenseLoop));

    void Update()
    {
        // Usa a velocidade do NavMeshAgent (plano XZ) para animação e flip
        Vector3 vel = Vector3.zero;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            vel = agent.velocity;
        }
        else
        {
            // fallback: calcula por posição (caso o agente esteja parado/desligado)
            vel = (transform.position - lastPosition) / Mathf.Max(Time.deltaTime, 0.0001f);
        }

        // Velocidade 2D considerando XZ (top-down 2D com NavMesh)
        Vector2 vel2D = new Vector2(vel.x, vel.z);
        float speed = vel2D.magnitude;

        bool isMoving = speed > 0.05f && state == State.Chase;
        animator.SetBool("isRunning", isMoving);

        // Flip do sprite apenas se estiver se movendo horizontalmente
        if (state == State.Chase && Mathf.Abs(vel2D.x) > 0.01f)
        {
            if (vel2D.x < 0f)
                transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
            else if (vel2D.x > 0f)
                transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }

        lastPosition = transform.position;
    }

    void SenseLoop()
    {
        if (!target) return;

        // Distância no plano XZ (NavMesh 2D usa XZ)
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
        SetAgentStopped(false);
        if (agent.enabled && agent.isOnNavMesh && target)
            agent.SetDestination(target.position);
        animator.SetBool("isRunning", true);
    }

    void GoDormant()
    {
        state = State.Dormant;
        SetAgentStopped(true);
        animator.SetBool("isRunning", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green; Gizmos.DrawWireSphere(transform.position, wakeRadius);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, sleepRadius);
    }

    // Helper para evitar exceção "Stop can only be called on an active agent that has been placed on a NavMesh"
    private void SetAgentStopped(bool stopped)
    {
        if (agent == null) return;
        if (!agent.enabled) return;
#if UNITY_2022_2_OR_NEWER
        if (!agent.isOnNavMesh) return;
#else
        if (!NavMesh.SamplePosition(agent.transform.position, out var _, 0.1f, NavMesh.AllAreas)) return;
#endif
        agent.isStopped = stopped;
    }
}
