using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        // fallback: tenta achar o player por Tag se você esquecer de arrastar no Inspector
        if (target == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) target = go.transform;
        }

        // reposiciona no NavMesh se nasceu na borda/fora
        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out var hit, 3f, NavMesh.AllAreas))
                agent.Warp(hit.position);
            else
            {
                Debug.LogError($"{name}: sem NavMesh próximo do spawn.");
            }
        }
    }

    void Update()
    {
        // evita NullReference e o erro do SetDestination
        if (agent != null && agent.enabled && agent.isOnNavMesh && target != null)
            agent.SetDestination(target.position);
    }
}
