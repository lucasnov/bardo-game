using UnityEngine;

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
            // morrer / destruir / tocar animação
            Destroy(gameObject);
        }
    }
}
