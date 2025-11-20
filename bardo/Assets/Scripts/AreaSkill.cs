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

    [Header("VFX da Habilidade")]
    public GameObject areaSkillPrefab;      // Prefab da animação da skill (só VFX)
    public float vfxDuration = 1.0f;        // Tempo para destruir o VFX
    public Vector3 vfxOffset = Vector3.zero; // Offset em relação ao player (ex.: (0, -0.5, 0))

    float nextReadyTime = 0f;

    // SFX
    public AudioSource AreaSkillSound;

    // Chame isso por input (ex.: botão de skill) ou via Animation Event
    public void Cast()
    {
        if (Time.time < nextReadyTime) return;
        nextReadyTime = Time.time + cooldown;

        // ---------- DANO EM ÁREA ----------
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);
        foreach (var h in hits)
        {
            // Procura um IDamageable no inimigo (no root ou no próprio collider)
            var dmg = h.GetComponentInParent<IDamageable>() ?? h.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }

        // ---------- VFX DA SKILL ----------
        if (areaSkillPrefab != null)
        {
            // Posição do VFX (no pé / centro do player + offset)
            Vector3 spawnPos = transform.position + vfxOffset;

            GameObject vfx = Instantiate(areaSkillPrefab, spawnPos, Quaternion.identity);

            // Opcional: fazer o VFX "seguir" o player enquanto toca
            vfx.transform.SetParent(transform);

            // Destroi o VFX depois que a animação terminar
            Destroy(vfx, vfxDuration);
        }
        else
        {
            Debug.LogWarning("[AreaSkill] Nenhum prefab de VFX atribuído em 'areaSkillPrefab'.");
        }

        // SFX
        AreaSkillSound.Play();
    }

    // Gizmo para visualizar o raio na cena
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
