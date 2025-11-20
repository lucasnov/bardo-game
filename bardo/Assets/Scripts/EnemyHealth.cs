using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHP = 100f;
    public float currentHP;

    void Awake() => currentHP = maxHP;

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0f)
        {
            // morrer / destruir / tocar anima��o
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Avoid running during editor recompiles or scene unloads
        if (!Application.isPlaying) return;

        // Notify WaveSpawner that this enemy has died
        WaveSpawner spawner = FindObjectOfType<WaveSpawner>();
        if (spawner != null)
        {
            spawner.OnEnemyDeath(gameObject);
        }
    }
}
