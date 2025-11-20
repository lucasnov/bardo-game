using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    public float speed = 5f;
    public float damage = 10f;

    [Header("Limites do Projétil")]
    public float maxDistance = 10f;   // distância máxima que o projétil pode viajar
    public float lifeTime = 3f;       // tempo máximo de vida do projétil

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        // Destroi automaticamente após X segundos, mesmo sem colidir
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;

        // Verifica distância percorrida
        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se atingir inimigo
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        // Se atingir qualquer coisa que não seja o Player
        else if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
        }
    }
}
