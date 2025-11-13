using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount);
}

public class AreaSkill : MonoBehaviour
{
    [Header("Config")]
    public float radius = 2.5f;
    public float damage = 25f;
    public LayerMask enemyMask;
    public float cooldown = 1.0f;

    float nextReadyTime = 0f;

    // Chame isso por input (ex.: botão de skill) ou via animação/evento
    public void Cast()
    {
        if (Time.time < nextReadyTime) return;
        nextReadyTime = Time.time + cooldown;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        foreach (var h in hits)
        {
            // Procura um IDamageable no inimigo (no root ou no próprio collider)
            var dmg = h.GetComponentInParent<IDamageable>() ?? h.GetComponent<IDamageable>();
            if (dmg != null) dmg.TakeDamage(damage);
        }

        // TODO: spawn VFX/SFX aqui
    }

    // Gizmo para visualizar o raio na cena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
